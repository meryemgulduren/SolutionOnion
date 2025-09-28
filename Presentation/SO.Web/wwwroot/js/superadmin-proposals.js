// SuperAdmin Proposals Management System
class SuperAdminProposalsManager {
    constructor() {
        this.currentPage = 1;
        this.pageSize = 10;
        this.allProposals = [];
        this.filteredProposals = [];
        this.searchTerm = '';
        this.statusFilter = '';
        this.userFilter = '';
    }

    init() {
        this.loadProposals();
        this.setupEventListeners();
    }

    setupEventListeners() {
        $('#searchInput').on('input', () => {
            this.searchTerm = $('#searchInput').val().toLowerCase();
            this.applyFilters();
        });

        $('#searchBtn').on('click', () => {
            this.searchTerm = $('#searchInput').val().toLowerCase();
            this.applyFilters();
        });

        $('#statusFilter').on('change', () => {
            this.statusFilter = $('#statusFilter').val();
            this.applyFilters();
        });

        $('#userFilter').on('change', () => {
            this.userFilter = $('#userFilter').val();
            this.applyFilters();
        });

        $('#clearFilters').on('click', () => {
            this.clearFilters();
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
            console.log('Loading all proposals for SuperAdmin...');
            const response = await fetch('/SuperAdmin/GetAllProposals');
            console.log('Response status:', response.status);
            
            if (response.ok) {
                const data = await response.json();
                console.log('API Response:', data);
                
                if (Array.isArray(data) && data.length > 0) {
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
                this.showAlert('Error loading proposals: ' + errorText, 'danger');
            }
        } catch (error) {
            console.error('Error loading proposals:', error);
            this.showAlert('Error loading proposals: ' + error.message, 'danger');
        }
    }

    populateUserFilter() {
        const users = [...new Set(this.allProposals.map(p => p.createdBy))];
        const userFilter = $('#userFilter');
        userFilter.empty();
        userFilter.append('<option value="">All Users</option>');
        users.forEach(user => {
            userFilter.append(`<option value="${user}">${user}</option>`);
        });
    }

    applyFilters() {
        this.filteredProposals = this.allProposals.filter(proposal => {
            const matchesSearch = !this.searchTerm || 
                proposal.proposalName.toLowerCase().includes(this.searchTerm) ||
                proposal.companyName.toLowerCase().includes(this.searchTerm) ||
                proposal.createdBy.toLowerCase().includes(this.searchTerm);
            
            const matchesStatus = !this.statusFilter || proposal.status === this.statusFilter;
            const matchesUser = !this.userFilter || proposal.createdBy === this.userFilter;
            
            return matchesSearch && matchesStatus && matchesUser;
        });
        
        this.currentPage = 1;
        this.displayProposals();
        this.updatePagination();
    }

    clearFilters() {
        $('#searchInput').val('');
        $('#statusFilter').val('');
        $('#userFilter').val('');
        this.searchTerm = '';
        this.statusFilter = '';
        this.userFilter = '';
        this.applyFilters();
    }

    displayProposals() {
        const tbody = $('#proposalsTableBody');
        tbody.empty();

        if (this.filteredProposals.length === 0) {
            tbody.append(`
                <tr>
                    <td colspan="7" class="text-center py-4">
                        <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
                        <p class="text-muted">No proposals found</p>
                    </td>
                </tr>
            `);
            return;
        }

        const startIndex = (this.currentPage - 1) * this.pageSize;
        const endIndex = startIndex + this.pageSize;
        const pageProposals = this.filteredProposals.slice(startIndex, endIndex);

        pageProposals.forEach(proposal => {
            const statusBadge = this.getStatusBadge(proposal.status);
            const row = `
                <tr data-id="${proposal.id}" class="hover-lift">
                    <td>
                        <div class="d-flex align-items-center">
                            <i class="fas fa-file-contract text-primary me-2"></i>
                            <strong>${this.escapeHtml(proposal.proposalName)}</strong>
                        </div>
                    </td>
                    <td>
                        <div class="d-flex align-items-center">
                            <i class="fas fa-building text-success me-2"></i>
                            ${this.escapeHtml(proposal.companyName)}
                        </div>
                    </td>
                    <td>
                        <div class="d-flex align-items-center">
                            <i class="fas fa-user text-info me-2"></i>
                            ${this.escapeHtml(proposal.createdBy)}
                        </div>
                    </td>
                    <td>
                        <div class="d-flex align-items-center">
                            <i class="fas fa-calendar text-warning me-2"></i>
                            ${this.formatDate(proposal.proposalDate)}
                        </div>
                    </td>
                    <td>${statusBadge}</td>
                    <td>
                        <div class="d-flex align-items-center">
                            <i class="fas fa-dollar-sign text-success me-2"></i>
                            $${this.formatNumber(proposal.totalAmount)}
                        </div>
                    </td>
                    <td class="text-center">
                        <div class="btn-group" role="group">
                            <a href="/Proposals/Edit/${proposal.id}" class="btn btn-sm btn-primary" title="Edit" onclick="console.log('Edit clicked for proposal:', '${proposal.id}')">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a href="/Proposals/Details/${proposal.id}" class="btn btn-sm btn-info" title="View Details" onclick="console.log('Details clicked for proposal:', '${proposal.id}')">
                                <i class="fas fa-eye"></i>
                            </a>
                            <button class="btn btn-sm btn-danger" onclick="superAdminProposalsManager.deleteProposal('${proposal.id}')" title="Delete">
                                <i class="fas fa-trash"></i>
                            </button>
                            <button class="btn btn-sm btn-success" onclick="superAdminProposalsManager.exportProposalToWord('${proposal.id}')" title="Export to Word">
                                <i class="fas fa-file-word"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });

        $('#proposalCount').text(this.filteredProposals.length);
    }

    getStatusBadge(status) {
        const statusClasses = {
            'Draft': 'badge-warning',
            'Active': 'badge-success',
            'Completed': 'badge-info',
            'Cancelled': 'badge-danger'
        };
        
        const statusTexts = {
            'Draft': 'Draft',
            'Active': 'Active',
            'Completed': 'Completed',
            'Cancelled': 'Cancelled'
        };
        
        const className = statusClasses[status] || 'badge-secondary';
        const text = statusTexts[status] || status;
        
        return `<span class="badge ${className}">${text}</span>`;
    }

    formatDate(date) {
        return new Date(date).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }

    formatNumber(number) {
        return new Intl.NumberFormat('en-US').format(number);
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    updatePagination() {
        const totalPages = Math.ceil(this.filteredProposals.length / this.pageSize);
        const pagination = $('#pagination');
        pagination.empty();

        if (totalPages <= 1) {
            $('#paginationInfo').text(`Showing ${this.filteredProposals.length} entries`);
            return;
        }

        // Previous button
        pagination.append(`
            <li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="superAdminProposalsManager.goToPage(${this.currentPage - 1})">Previous</a>
            </li>
        `);

        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || (i >= this.currentPage - 2 && i <= this.currentPage + 2)) {
                pagination.append(`
                    <li class="page-item ${i === this.currentPage ? 'active' : ''}">
                        <a class="page-link" href="#" onclick="superAdminProposalsManager.goToPage(${i})">${i}</a>
                    </li>
                `);
            } else if (i === this.currentPage - 3 || i === this.currentPage + 3) {
                pagination.append('<li class="page-item disabled"><span class="page-link">...</span></li>');
            }
        }

        // Next button
        pagination.append(`
            <li class="page-item ${this.currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="superAdminProposalsManager.goToPage(${this.currentPage + 1})">Next</a>
            </li>
        `);

        const startIndex = (this.currentPage - 1) * this.pageSize + 1;
        const endIndex = Math.min(this.currentPage * this.pageSize, this.filteredProposals.length);
        $('#paginationInfo').text(`Showing ${startIndex} to ${endIndex} of ${this.filteredProposals.length} entries`);
    }

    goToPage(page) {
        const totalPages = Math.ceil(this.filteredProposals.length / this.pageSize);
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
            // Get CSRF token
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

    async exportProposalToWord(proposalId) {
        try {
            const response = await fetch(`/SuperAdmin/ExportProposalWord/${proposalId}`);
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `proposal-${proposalId}.docx`;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                this.showAlert('Proposal exported to Word successfully', 'success');
            } else {
                this.showAlert('Error exporting proposal to Word', 'danger');
            }
        } catch (error) {
            console.error('Error exporting proposal to Word:', error);
            this.showAlert('Error exporting proposal to Word', 'danger');
        }
    }

    async exportToWord() {
        try {
            const response = await fetch('/SuperAdmin/ExportAllWord');
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = 'all-proposals.docx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                this.showAlert('All proposals exported to Word successfully', 'success');
            } else {
                this.showAlert('Error exporting proposals to Word', 'danger');
            }
        } catch (error) {
            console.error('Error exporting proposals to Word:', error);
            this.showAlert('Error exporting proposals to Word', 'danger');
        }
    }

    async exportToExcel() {
        try {
            const response = await fetch('/SuperAdmin/ExportAllExcel');
            if (response.ok) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = 'all-proposals.xlsx';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
                this.showAlert('All proposals exported to Excel successfully', 'success');
            } else {
                this.showAlert('Error exporting proposals to Excel', 'danger');
            }
        } catch (error) {
            console.error('Error exporting proposals to Excel:', error);
            this.showAlert('Error exporting proposals to Excel', 'danger');
        }
    }

    showAlert(message, type) {
        const alertContainer = $('#alertContainer');
        const alertId = 'alert-' + Date.now();
        const alertHtml = `
            <div id="${alertId}" class="alert alert-${type} alert-dismissible fade show" role="alert">
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        alertContainer.html(alertHtml);
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            $(`#${alertId}`).alert('close');
        }, 5000);
    }
}

// Global instance
let superAdminProposalsManager;
