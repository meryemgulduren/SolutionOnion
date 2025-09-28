// Accounts Management System
class AccountsManager {
    constructor() {
        this.baseUrl = '/Accounts';
        this.currentPage = 1;
        this.pageSize = 10;
        this.totalCount = 0;
        this.filteredData = [];
        this.initializeEventListeners();
        this.initializeFilters();
    }

    initializeEventListeners() {
        // Load accounts on page load
        if ($('#accountsTable').length) {
            this.loadAccounts();
        }

        // Search functionality
        $('#searchInput').on('input', this.debounce(() => this.applyFilters(), 300));
        $('#searchBtn').on('click', () => this.applyFilters());
        
        // Filter changes
        $('#statusFilter, #sortBy').on('change', () => this.applyFilters());
        
        // Clear filters
        $('#clearFilters').on('click', () => this.clearFilters());
        
        // Refresh button
        $('#refreshBtn').on('click', () => this.loadAccounts());
        
        // Export button
        $('#exportClientsBtn').on('click', () => this.exportClients());
        
        // Pagination
        $('#prevPage').on('click', () => this.previousPage());
        $('#nextPage').on('click', () => this.nextPage());

        // Create account form
        $('#createAccountForm').on('submit', (e) => this.createAccount(e));

        // Edit account form
        $('#editAccountForm').on('submit', (e) => this.updateAccount(e));

        // View details buttons
        $(document).on('click', '.view-account-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.viewAccountDetails(id);
        });

        // Edit buttons
        $(document).on('click', '.edit-account-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.editAccount(id);
        });

        // Delete buttons
        $(document).on('click', '.delete-account-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.showDeleteConfirmation(id);
        });

        // Edit from details modal
        $('#editFromDetails').on('click', () => {
            const id = $('#accountDetailsModal').data('account-id');
            if (id) {
                this.editAccount(id);
            }
        });
    }

    initializeFilters() {
        // Set default values
        $('#statusFilter').val('');
        $('#sortBy').val('companyName');
        $('#searchInput').val('');
    }

    // Load all accounts
    async loadAccounts() {
        try {
            this.showLoading(true);
            const response = await $.ajax({
                url: `${this.baseUrl}/GetAllAccounts`,
                type: 'GET',
                dataType: 'json'
            });
            
            this.filteredData = response || [];
            this.totalCount = this.filteredData.length;
            this.currentPage = 1;
            this.applyFilters();
        } catch (error) {
            this.showAlert('Error loading accounts: ' + error.message, 'danger');
        } finally {
            this.showLoading(false);
        }
    }

    // Apply search and filters
    applyFilters() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const statusFilter = $('#statusFilter').val();
        const sortBy = $('#sortBy').val();

        let filtered = this.filteredData.filter(account => {
            // Search filter
            const matchesSearch = !searchTerm || 
                account.companyName.toLowerCase().includes(searchTerm) ||
                (account.contactPerson && account.contactPerson.toLowerCase().includes(searchTerm)) ||
                account.email.toLowerCase().includes(searchTerm);

            // Status filter
            const matchesStatus = !statusFilter || account.isActive.toString() === statusFilter;

            return matchesSearch && matchesStatus;
        });

        // Sort data
        filtered = this.sortData(filtered, sortBy);

        this.displayAccounts(filtered);
        this.updatePagination(filtered.length);
    }

    // Sort data based on selected criteria
    sortData(data, sortBy) {
        return data.sort((a, b) => {
            switch (sortBy) {
                case 'companyName':
                    return a.companyName.localeCompare(b.companyName);
                case 'createdDate':
                    return new Date(b.createdDate) - new Date(a.createdDate);
                case 'contactPerson':
                    const personA = a.contactPerson || '';
                    const personB = b.contactPerson || '';
                    return personA.localeCompare(personB);
                default:
                    return 0;
            }
        });
    }

    // Clear all filters
    clearFilters() {
        $('#searchInput').val('');
        $('#statusFilter').val('');
        $('#sortBy').val('companyName');
        this.applyFilters();
    }

    // Display accounts in table
    displayAccounts(accounts) {
        const tbody = $('#accountsTable tbody');
        tbody.empty();

        if (accounts && accounts.length > 0) {
            const startIndex = (this.currentPage - 1) * this.pageSize;
            const endIndex = startIndex + this.pageSize;
            const pageAccounts = accounts.slice(startIndex, endIndex);

            pageAccounts.forEach(account => {
                const statusBadge = this.getStatusBadge(account.isActive);
                const row = `
                    <tr data-id="${account.id}" class="hover-lift">
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-building text-primary me-2"></i>
                                <strong>${this.escapeHtml(account.companyName)}</strong>
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-user text-success me-2"></i>
                                ${this.escapeHtml(account.contactPerson || 'N/A')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-envelope text-info me-2"></i>
                                <a href="mailto:${account.email}" class="text-decoration-none">
                                    ${this.escapeHtml(account.email)}
                                </a>
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-phone text-warning me-2"></i>
                                ${this.escapeHtml(account.phoneNumber || 'N/A')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-map-marker-alt text-secondary me-2"></i>
                                ${this.escapeHtml(account.taxOffice || 'N/A')}
                            </div>
                        </td>
                        <td>${statusBadge}</td>
                        <td class="text-center">
                            <div class="action-buttons">
                                <button class="btn btn-outline-info btn-sm view-account-btn" 
                                        data-id="${account.id}" title="View Details">
                                    <i class="fas fa-eye"></i>
                                </button>
                                <button class="btn btn-outline-warning btn-sm edit-account-btn" 
                                        data-id="${account.id}" title="Edit Client">
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button class="btn btn-outline-danger btn-sm delete-account-btn" 
                                        data-id="${account.id}" title="Delete Client">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                `;
                tbody.append(row);
            });
        } else {
            tbody.append(`
                <tr>
                    <td colspan="7" class="text-center">
                        <div class="empty-state py-4">
                            <i class="fas fa-search fa-2x text-muted mb-2"></i>
                            <p class="text-muted mb-0">No clients found matching your criteria.</p>
                        </div>
                    </td>
                </tr>
            `);
        }

        this.updateCounts(accounts.length);
    }

    // Update pagination controls
    updatePagination(totalFiltered) {
        const totalPages = Math.ceil(totalFiltered / this.pageSize);
        const startItem = (this.currentPage - 1) * this.pageSize + 1;
        const endItem = Math.min(this.currentPage * this.pageSize, totalFiltered);

        $('#showingCount').text(`${startItem}-${endItem}`);
        $('#totalCount').text(totalFiltered);

        $('#prevPage').prop('disabled', this.currentPage === 1);
        $('#nextPage').prop('disabled', this.currentPage >= totalPages);
    }

    // Previous page
    previousPage() {
        if (this.currentPage > 1) {
            this.currentPage--;
            this.applyFilters();
        }
    }

    // Next page
    nextPage() {
        const totalPages = Math.ceil(this.totalCount / this.pageSize);
        if (this.currentPage < totalPages) {
            this.currentPage++;
            this.applyFilters();
        }
    }

    // Update counts display
    updateCounts(filteredCount) {
        this.totalCount = filteredCount;
        this.updatePagination(filteredCount);
    }

    // Get status badge HTML
    getStatusBadge(isActive) {
        const statusClass = isActive ? 'status-approved' : 'status-cancelled';
        const statusText = isActive ? 'Active' : 'Inactive';
        
        return `<span class="status-badge ${statusClass}">${statusText}</span>`;
    }

    async createAccount(e) {
        e.preventDefault();

        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const mappedData = {
            companyName: data.CompanyName,
            contactPerson: data.ContactPerson,
            email: data.Email,
            phoneNumber: data.PhoneNumber,
            taxOffice: data.TaxOffice,
            taxNumber: data.TaxNumber,
            isActive: data.IsActive === 'on'
        };

        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/Create`, // /Accounts/Create
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(mappedData),
                dataType: 'json',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                }
            });

            $('#createAccountModal').modal('hide');
            this.showAlert(response.message || 'Client created!', 'success');
            this.loadAccounts();
            e.target.reset();
        } catch (error) {
            const msg = error.responseJSON?.message || error.message || "Bilinmeyen hata";
            this.showAlert('Error creating client: ' + msg, 'danger');
        }
    }


    // Edit account
    editAccount(id) {
        const account = this.filteredData.find(a => a.id === id);
        if (account) {
            this.populateEditForm(account);
            $('#editAccountModal').modal('show');
        }
    }

    // Populate edit form
    populateEditForm(account) {
        $('#EditId').val(account.id);
        $('#EditCompanyName').val(account.companyName);
        $('#EditContactPerson').val(account.contactPerson || '');
        $('#EditEmail').val(account.email);
        $('#EditPhoneNumber').val(account.phoneNumber || '');
        $('#EditTaxOffice').val(account.taxOffice || '');
        $('#EditTaxNumber').val(account.taxNumber || '');
        $('#EditIsActive').prop('checked', account.isActive);
    }

    // Update account
    async updateAccount(e) {
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());
        
        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/Update`,
                type: 'POST',
                data: data,
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                }
            });

            $('#editAccountModal').modal('hide');
            this.showAlert('Client updated successfully!', 'success');
            this.loadAccounts();
        } catch (error) {
            this.showAlert('Error updating client: ' + error.message, 'danger');
        }
    }

    // View account details
    async viewAccountDetails(id) {
        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/GetById/${id}`,
                type: 'GET',
                dataType: 'json'
            });
            
            this.populateDetailsModal(response);
            $('#accountDetailsModal').modal('show');
            $('#accountDetailsModal').data('account-id', id);
        } catch (error) {
            this.showAlert('Error loading client details: ' + error.message, 'danger');
        }
    }

    // Populate details modal
    populateDetailsModal(data) {
        $('#DetailsCompanyName').text(data.companyName || 'N/A');
        $('#DetailsContactPerson').text(data.contactPerson || 'N/A');
        $('#DetailsEmail').text(data.email || 'N/A');
        $('#DetailsPhoneNumber').text(data.phoneNumber || 'N/A');
        $('#DetailsTaxOffice').text(data.taxOffice || 'N/A');
        $('#DetailsTaxNumber').text(data.taxNumber || 'N/A');
        $('#DetailsIsActive').html(this.getStatusBadge(data.isActive));
        $('#DetailsCreatedDate').text(data.createdDate ? 
            new Date(data.createdDate).toLocaleDateString('tr-TR') : 'N/A');
    }

    // Show delete confirmation
    showDeleteConfirmation(id) {
        $('#deleteConfirmModal').modal('show');
        $('#confirmDelete').off('click').on('click', () => this.deleteAccount(id));
    }

    // Delete account
    async deleteAccount(id) {
        try {
            const token = $('input[name="__RequestVerificationToken"]').val();

            await $.ajax({
                url: `${this.baseUrl}/Delete`,
                type: 'POST',
                data: { id: id },
                headers: {
                    'RequestVerificationToken': token
                }
            });

            $('#deleteConfirmModal').modal('hide');
            this.showAlert('Client deleted successfully!', 'success');
            this.loadAccounts();
        } catch (error) {
            const msg = error.responseJSON?.message || error.message;
            this.showAlert('Error deleting client: ' + msg, 'danger');
        }
    }


    // Export clients
    async exportClients() {
        try {
            this.showAlert('Exporting clients...', 'info');
            // Implementation for exporting clients
            // This would typically call an export endpoint
        } catch (error) {
            this.showAlert('Error exporting clients: ' + error.message, 'danger');
        }
    }

    // Show loading spinner
    showLoading(show) {
        if (show) {
            $('#loadingSpinner').show();
            $('#accountsTable').hide();
        } else {
            $('#loadingSpinner').hide();
            $('#accountsTable').show();
        }
    }

    // Show alert message
    showAlert(message, type) {
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                <i class="fas fa-${this.getAlertIcon(type)} me-2"></i>
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        
        $('#alertContainer').html(alertHtml);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            $('.alert').fadeOut();
        }, 5000);
    }

    // Get alert icon based on type
    getAlertIcon(type) {
        switch (type) {
            case 'success': return 'check-circle';
            case 'danger': return 'exclamation-triangle';
            case 'warning': return 'exclamation-triangle';
            case 'info': return 'info-circle';
            default: return 'info-circle';
        }
    }

    // Escape HTML to prevent XSS
    escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Debounce function for search input
    debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
}

// Initialize when document is ready
$(document).ready(function() {
    new AccountsManager();
}); 