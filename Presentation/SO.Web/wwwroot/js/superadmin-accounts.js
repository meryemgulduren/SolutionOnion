class SuperAdminAccountsManager {
    constructor() {
        this.allAccounts = [];
        this.filteredAccounts = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;
        this.currentFilter = 'all';
        this.currentUserFilter = 'all';
        this.searchTerm = '';
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadAccounts();
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
    }

    async loadAccounts() {
        try {
            console.log('Loading all accounts for SuperAdmin...');
            const response = await fetch('/SuperAdmin/GetAllAccounts');
            console.log('Response status:', response.status);
            
            if (response.ok) {
                const data = await response.json();
                console.log('API Response:', data);
                
                if (data && data.length > 0) {
                    this.allAccounts = data.map(account => ({
                        id: account.id,
                        companyName: account.companyName || 'Unnamed Company',
                        contactPerson: account.contactPerson || 'No Contact',
                        email: account.email || 'No Email',
                        phone: account.phone || 'No Phone',
                        createdBy: account.createdBy || 'Unknown',
                        createdDate: new Date(account.createdDate),
                        status: account.status || 'Active'
                    }));
                    
                    this.filteredAccounts = [...this.allAccounts];
                    this.populateUserFilter();
                    this.displayAccounts();
                    this.updatePagination();
                    console.log('Loaded accounts:', this.allAccounts.length);
                } else {
                    this.showAlert('No accounts found', 'info');
                    this.allAccounts = [];
                    this.filteredAccounts = [];
                    this.displayAccounts();
                }
            } else {
                const errorText = await response.text();
                console.error('API Error:', errorText);
                this.showAlert('Error loading accounts', 'danger');
            }
        } catch (error) {
            console.error('Error loading accounts:', error);
            this.showAlert('Error loading accounts', 'danger');
        }
    }

    populateUserFilter() {
        const userFilter = $('#userFilter');
        const uniqueUsers = [...new Set(this.allAccounts.map(a => a.createdBy))];
        
        userFilter.empty();
        userFilter.append('<option value="all">All Users</option>');
        
        uniqueUsers.forEach(user => {
            userFilter.append(`<option value="${user}">${user}</option>`);
        });
    }

    applyFilters() {
        this.filteredAccounts = this.allAccounts.filter(account => {
            const matchesSearch = account.companyName.toLowerCase().includes(this.searchTerm) ||
                                account.contactPerson.toLowerCase().includes(this.searchTerm) ||
                                account.email.toLowerCase().includes(this.searchTerm);
            const matchesStatus = this.currentFilter === 'all' || account.status === this.currentFilter;
            const matchesUser = this.currentUserFilter === 'all' || account.createdBy === this.currentUserFilter;
            
            return matchesSearch && matchesStatus && matchesUser;
        });
        
        this.currentPage = 1;
        this.displayAccounts();
        this.updatePagination();
    }

    displayAccounts() {
        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        const endIndex = startIndex + this.itemsPerPage;
        const accountsToShow = this.filteredAccounts.slice(startIndex, endIndex);
        
        const tbody = $('#accountsTableBody');
        tbody.empty();
        
        if (accountsToShow.length === 0) {
            tbody.append(`
                <tr>
                    <td colspan="8" class="text-center py-4">
                        <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                        <p class="text-muted mb-0">No accounts found</p>
                    </td>
                </tr>
            `);
            return;
        }
        
        accountsToShow.forEach(account => {
            const row = `
                <tr>
                    <td>${account.companyName}</td>
                    <td>${account.contactPerson}</td>
                    <td>${account.email}</td>
                    <td>${account.phone}</td>
                    <td>${account.createdBy}</td>
                    <td>${account.createdDate.toLocaleDateString()}</td>
                    <td>
                        <span class="badge bg-${this.getStatusColor(account.status)}">
                            ${account.status}
                        </span>
                    </td>
                    <td>
                        <div class="btn-group" role="group">
                            <a href="/Accounts/Edit/${account.id}" class="btn btn-sm btn-primary" title="Edit" onclick="console.log('Edit clicked for account:', '${account.id}')">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a href="/Accounts/Details/${account.id}" class="btn btn-sm btn-info" title="View Details" onclick="console.log('Details clicked for account:', '${account.id}')">
                                <i class="fas fa-eye"></i>
                            </a>
                            <button class="btn btn-sm btn-danger" title="Delete" onclick="superAdminAccountsManager.deleteAccount('${account.id}')">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
        
        // Update count
        $('#accountCount').text(`${this.filteredAccounts.length} clients`);
    }

    getStatusColor(status) {
        const colors = {
            'Active': 'success',
            'Inactive': 'secondary',
            'Pending': 'warning'
        };
        return colors[status] || 'secondary';
    }

    updatePagination() {
        const totalPages = Math.ceil(this.filteredAccounts.length / this.itemsPerPage);
        const pagination = $('#pagination');
        pagination.empty();
        
        if (totalPages <= 1) return;
        
        // Previous button
        pagination.append(`
            <li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="superAdminAccountsManager.goToPage(${this.currentPage - 1})">Previous</a>
            </li>
        `);
        
        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            pagination.append(`
                <li class="page-item ${i === this.currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="superAdminAccountsManager.goToPage(${i})">${i}</a>
                </li>
            `);
        }
        
        // Next button
        pagination.append(`
            <li class="page-item ${this.currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="superAdminAccountsManager.goToPage(${this.currentPage + 1})">Next</a>
            </li>
        `);
    }

    goToPage(page) {
        const totalPages = Math.ceil(this.filteredAccounts.length / this.itemsPerPage);
        if (page >= 1 && page <= totalPages) {
            this.currentPage = page;
            this.displayAccounts();
            this.updatePagination();
        }
    }

    async deleteAccount(accountId) {
        if (!confirm('Are you sure you want to delete this account?')) {
            return;
        }
        
        try {
            const token = $('input[name="__RequestVerificationToken"]').val();
            
            // FormData kullanarak POST request gönder
            const formData = new FormData();
            formData.append('id', accountId);
            formData.append('__RequestVerificationToken', token);
            
            const response = await fetch('/Accounts/Delete', {
                method: 'POST',
                body: formData
            });
            
            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.showAlert('Account deleted successfully', 'success');
                    this.loadAccounts();
                } else {
                    this.showAlert(result.message || 'Error deleting account', 'danger');
                }
            } else {
                const errorText = await response.text();
                console.error('Delete error:', errorText);
                this.showAlert('Error deleting account: ' + errorText, 'danger');
            }
        } catch (error) {
            console.error('Error deleting account:', error);
            this.showAlert('Error deleting account: ' + error.message, 'danger');
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
    window.superAdminAccountsManager = new SuperAdminAccountsManager();
});
