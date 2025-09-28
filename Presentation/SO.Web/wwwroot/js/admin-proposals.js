class AdminProposalsManager {
    constructor() {
        this.allProposals = [];
        this.filteredProposals = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;
        this.currentFilter = 'all';
        this.currentUserFilter = 'all';
        this.searchTerm = '';
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadProposals();
    }

    bindEvents() {
        $('#searchInput').on('input', (e) => {
            this.searchTerm = e.target.value.toLowerCase();
            this.applyFilters();
        });

        $('#statusFilter').on('change', (e) => {
            this.currentFilter = e.target.value;
            this.applyFilters();
        });

        $('#userFilter').on('change', (e) => {
            this.currentUserFilter = e.target.value;
            this.applyFilters();
        });

        $('#exportAllWordBtn').on('click', () => {
            this.exportToWord();
        });

        $('#exportAllExcelBtn').on('click', () => {
            this.exportToExcel();
        });
    }

    async loadProposals() {
        try {
            console.log('Loading all proposals for Admin...');
            const response = await fetch('/Admin/GetAllProposals');
            console.log('Response status:', response.status);
            
            if (response.ok) {
                const data = await response.json();
                console.log('API Response:', data);
                
                if (data && data.length > 0) {
                    this.allProposals = data.map(proposal => ({
                        id: proposal.id,
                        proposalName: proposal.proposalName || 'Unnamed Proposal',
                        companyName: proposal.companyName || 'No Company',
                        createdBy: proposal.preparedBy || 'Unknown',
                        proposalDate: new Date(proposal.proposalDate),
                        status: proposal.status || 'Draft',
                        totalAmount: proposal.totalAmount || 0,
                        preparedBy: proposal.preparedBy || 'Unknown'
                    }));
                    
                    this.filteredProposals = [...this.allProposals];
                    this.populateUserFilter();
                    this.displayProposals();
                    this.updatePagination();
                    console.log('Loaded proposals:', this.allProposals.length);
                } else {
                    this.showAlert('No proposals found', 'info');
                    this.allProposals = [];
                    this.filteredProposals = [];
                    this.displayProposals();
                }
            } else {
                const errorText = await response.text();
                console.error('API Error:', errorText);
                this.showAlert('Error loading proposals', 'danger');
            }
        } catch (error) {
            console.error('Error loading proposals:', error);
            this.showAlert('Error loading proposals', 'danger');
        }
    }

    populateUserFilter() {
        const userFilter = $('#userFilter');
        const uniqueUsers = [...new Set(this.allProposals.map(p => p.preparedBy))];
        
        userFilter.empty();
        userFilter.append('<option value="all">All Users</option>');
        
        uniqueUsers.forEach(user => {
            userFilter.append(`<option value="${user}">${user}</option>`);
        });
    }

    applyFilters() {
        this.filteredProposals = this.allProposals.filter(proposal => {
            const matchesSearch = proposal.proposalName.toLowerCase().includes(this.searchTerm) ||
                                proposal.companyName.toLowerCase().includes(this.searchTerm);
            const matchesStatus = this.currentFilter === 'all' || proposal.status === this.currentFilter;
            const matchesUser = this.currentUserFilter === 'all' || proposal.preparedBy === this.currentUserFilter;
            
            return matchesSearch && matchesStatus && matchesUser;
        });
        
        this.currentPage = 1;
        this.displayProposals();
        this.updatePagination();
    }

    displayProposals() {
        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        const endIndex = startIndex + this.itemsPerPage;
        const proposalsToShow = this.filteredProposals.slice(startIndex, endIndex);
        
        const tbody = $('#proposalsTableBody');
        tbody.empty();
        
        if (proposalsToShow.length === 0) {
            tbody.append(`
                <tr>
                    <td colspan="7" class="text-center py-4">
                        <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                        <p class="text-muted mb-0">No proposals found</p>
                    </td>
                </tr>
            `);
            return;
        }
        
        proposalsToShow.forEach(proposal => {
            const row = `
                <tr>
                    <td>${proposal.proposalName}</td>
                    <td>${proposal.companyName}</td>
                    <td>${proposal.preparedBy}</td>
                    <td>${proposal.proposalDate.toLocaleDateString()}</td>
                    <td>
                        <span class="badge bg-${this.getStatusColor(proposal.status)}">
                            ${proposal.status}
                        </span>
                    </td>
                    <td>$${proposal.totalAmount.toLocaleString()}</td>
                    <td>
                        <div class="btn-group" role="group">
                            <a href="/Proposals/Edit/${proposal.id}" class="btn btn-sm btn-primary" title="Edit" onclick="console.log('Edit clicked for proposal:', '${proposal.id}')">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a href="/Proposals/Details/${proposal.id}" class="btn btn-sm btn-info" title="View Details" onclick="console.log('Details clicked for proposal:', '${proposal.id}')">
                                <i class="fas fa-eye"></i>
                            </a>
                            <button class="btn btn-sm btn-danger" title="Delete" onclick="adminProposalsManager.deleteProposal('${proposal.id}')">
                                <i class="fas fa-trash"></i>
                            </button>
                            <a href="/Admin/ExportProposalWord/${proposal.id}" class="btn btn-sm btn-success" title="Export Word">
                                <i class="fas fa-file-word"></i>
                            </a>
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
    }

    getStatusColor(status) {
        const colors = {
            'Draft': 'secondary',
            'Pending': 'warning',
            'Approved': 'success',
            'Rejected': 'danger',
            'In Review': 'info'
        };
        return colors[status] || 'secondary';
    }

    updatePagination() {
        const totalPages = Math.ceil(this.filteredProposals.length / this.itemsPerPage);
        const pagination = $('#pagination');
        pagination.empty();
        
        if (totalPages <= 1) return;
        
        // Previous button
        pagination.append(`
            <li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="adminProposalsManager.goToPage(${this.currentPage - 1})">Previous</a>
            </li>
        `);
        
        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            pagination.append(`
                <li class="page-item ${i === this.currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="adminProposalsManager.goToPage(${i})">${i}</a>
                </li>
            `);
        }
        
        // Next button
        pagination.append(`
            <li class="page-item ${this.currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="adminProposalsManager.goToPage(${this.currentPage + 1})">Next</a>
            </li>
        `);
    }

    goToPage(page) {
        const totalPages = Math.ceil(this.filteredProposals.length / this.itemsPerPage);
        if (page >= 1 && page <= totalPages) {
            this.currentPage = page;
            this.displayProposals();
            this.updatePagination();
        }
    }

    async deleteProposal(proposalId) {
        if (!confirm('Are you sure you want to delete this proposal?')) {
            return;
        }
        
        try {
            const token = $('input[name="__RequestVerificationToken"]').val();
            const response = await fetch(`/Proposals/Delete/${proposalId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                }
            });
            
            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.showAlert('Proposal deleted successfully', 'success');
                    this.loadProposals();
                } else {
                    this.showAlert(result.message || 'Error deleting proposal', 'danger');
                }
            } else {
                const errorText = await response.text();
                console.error('Delete error:', errorText);
                this.showAlert('Error deleting proposal: ' + errorText, 'danger');
            }
        } catch (error) {
            console.error('Error deleting proposal:', error);
            this.showAlert('Error deleting proposal: ' + error.message, 'danger');
        }
    }

    async exportToWord() {
        try {
            const proposalIds = this.filteredProposals.map(p => p.id);
            const response = await fetch('/Admin/ExportAllWord', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ proposalIds })
            });
            
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `All_Proposals_${new Date().toISOString().split('T')[0]}.docx`;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                this.showAlert('Word export completed successfully', 'success');
            } else {
                this.showAlert('Error exporting to Word', 'danger');
            }
        } catch (error) {
            console.error('Error exporting to Word:', error);
            this.showAlert('Error exporting to Word', 'danger');
        }
    }

    async exportToExcel() {
        try {
            const proposalIds = this.filteredProposals.map(p => p.id);
            const response = await fetch('/Admin/ExportAllExcel', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ proposalIds })
            });
            
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `All_Proposals_${new Date().toISOString().split('T')[0]}.xlsx`;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                this.showAlert('Excel export completed successfully', 'success');
            } else {
                this.showAlert('Error exporting to Excel', 'danger');
            }
        } catch (error) {
            console.error('Error exporting to Excel:', error);
            this.showAlert('Error exporting to Excel', 'danger');
        }
    }

    showAlert(message, type) {
        const alertDiv = $(`
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `);
        
        $('#alertContainer').html(alertDiv);
        
        setTimeout(() => {
            alertDiv.alert('close');
        }, 5000);
    }
}

// Initialize when document is ready
$(document).ready(function() {
    window.adminProposalsManager = new AdminProposalsManager();
});
