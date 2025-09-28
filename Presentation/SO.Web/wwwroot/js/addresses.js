// Addresses Management System
class AddressesManager {
    constructor() {
        this.baseUrl = '/Addresses';
        this.currentPage = 1;
        this.pageSize = 10;
        this.totalCount = 0;
        this.filteredData = [];
        this.initializeEventListeners();
        this.initializeFilters();
    }

    initializeEventListeners() {
        // Load addresses on page load
        if ($('#addressesTable').length) {
            this.loadAddresses();
        }

        // Search functionality
        $('#searchInput').on('input', this.debounce(() => this.applyFilters(), 300));
        $('#searchBtn').on('click', () => this.applyFilters());
        
        // Filter changes
        $('#clientFilter, #statusFilter').on('change', () => this.applyFilters());
        
        // Clear filters
        $('#clearFilters').on('click', () => this.clearFilters());
        
        // Refresh button
        $('#refreshBtn').on('click', () => this.loadAddresses());
        
        // Export button
        $('#exportAddressesBtn').on('click', () => this.exportAddresses());
        
        // Pagination
        $('#prevPage').on('click', () => this.previousPage());
        $('#nextPage').on('click', () => this.nextPage());

        // Create address form
        $('#createAddressForm').on('submit', (e) => this.createAddress(e));

        // Edit address form
        $('#editAddressForm').on('submit', (e) => this.updateAddress(e));

        // View details buttons
        $(document).on('click', '.view-address-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.viewAddressDetails(id);
        });

        // Edit buttons
        $(document).on('click', '.edit-address-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.editAddress(id);
        });

        // Delete buttons
        $(document).on('click', '.delete-address-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.showDeleteConfirmation(id);
        });

        // Edit from details modal
        $('#editFromDetails').on('click', () => {
            const id = $('#addressDetailsModal').data('address-id');
            if (id) {
                this.editAddress(id);
            }
        });
    }

    initializeFilters() {
        // Set default values
        $('#clientFilter').val('');
        $('#statusFilter').val('');
        $('#searchInput').val('');
    }

    // Load all addresses
    async loadAddresses() {
        try {
            this.showLoading(true);
            const response = await $.ajax({
                url: `${this.baseUrl}/GetAllAddresses`,
                type: 'GET',
                dataType: 'json'
            });
            
            this.filteredData = response || [];
            this.totalCount = this.filteredData.length;
            this.currentPage = 1;
            this.applyFilters();
        } catch (error) {
            this.showAlert('Error loading addresses: ' + error.message, 'danger');
        } finally {
            this.showLoading(false);
        }
    }

    // Apply search and filters
    applyFilters() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const clientFilter = $('#clientFilter').val();
        const statusFilter = $('#statusFilter').val();

        let filtered = this.filteredData.filter(address => {
            // Search filter
            const matchesSearch = !searchTerm || 
                address.addressName.toLowerCase().includes(searchTerm) ||
                address.city.toLowerCase().includes(searchTerm) ||
                (address.companyName && address.companyName.toLowerCase().includes(searchTerm));

            // Client filter
            const matchesClient = !clientFilter || address.accountId.toString() === clientFilter;

            // Status filter
            const matchesStatus = !statusFilter || address.active.toString() === statusFilter;

            return matchesSearch && matchesClient && matchesStatus;
        });

        this.displayAddresses(filtered);
        this.updatePagination(filtered.length);
    }

    // Clear all filters
    clearFilters() {
        $('#searchInput').val('');
        $('#clientFilter').val('');
        $('#statusFilter').val('');
        this.applyFilters();
    }

    // Display addresses in table
    displayAddresses(addresses) {
        const tbody = $('#addressesTable tbody');
        tbody.empty();

        if (addresses && addresses.length > 0) {
            const startIndex = (this.currentPage - 1) * this.pageSize;
            const endIndex = startIndex + this.pageSize;
            const pageAddresses = addresses.slice(startIndex, endIndex);

            pageAddresses.forEach(address => {
                const statusBadge = this.getStatusBadge(address.active);
                const defaultBadge = address.isDefault ? 
                    '<span class="badge bg-warning ms-1"><i class="fas fa-star me-1"></i>Default</span>' : '';
                
                const row = `
                    <tr data-id="${address.id}" class="hover-lift">
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-map-marker-alt text-primary me-2"></i>
                                <strong>${this.escapeHtml(address.addressName || 'N/A')}</strong>
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-building text-secondary me-2"></i>
                                ${this.escapeHtml(address.companyName || 'N/A')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-tag text-info me-2"></i>
                                ${this.escapeHtml(address.addressType || 'N/A')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-map text-success me-2"></i>
                                ${this.escapeHtml(address.addressLine1 || 'N/A')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-city text-warning me-2"></i>
                                ${this.escapeHtml(address.city || 'N/A')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-flag text-danger me-2"></i>
                                ${this.escapeHtml(address.country || 'N/A')}
                            </div>
                        </td>
                        <td>
                            ${statusBadge}${defaultBadge}
                        </td>
                        <td class="text-center">
                            <div class="action-buttons">
                                <button class="btn btn-outline-info btn-sm view-address-btn" 
                                        data-id="${address.id}" title="View Details">
                                    <i class="fas fa-eye"></i>
                                </button>
                                <button class="btn btn-outline-warning btn-sm edit-address-btn" 
                                        data-id="${address.id}" title="Edit Address">
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button class="btn btn-outline-danger btn-sm delete-address-btn" 
                                        data-id="${address.id}" title="Delete Address">
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
                    <td colspan="8" class="text-center">
                        <div class="empty-state py-4">
                            <i class="fas fa-search fa-2x text-muted mb-2"></i>
                            <p class="text-muted mb-0">No addresses found matching your criteria.</p>
                        </div>
                    </td>
                </tr>
            `);
        }

        this.updateCounts(addresses.length);
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

    // Create new address
    async createAddress(e) {
        e.preventDefault();

        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());

        const mappedData = {
            accountId: data.AccountId,
            isDefault: data.IsDefault === 'on',
            addressType: data.AddressType,
            addressName: data.AddressName,
            addressLine1: data.AddressLine1,
            addressLine2: data.AddressLine2,
            city: data.City,
            state: data.State,
            zip: data.Zip,
            country: data.Country,
            description: data.Description,
            phone: data.Phone,
            Fax: data.Fax,
            mail: data.Mail,
            active: data.Active === 'on'
        };

        try {
            const response = await $.ajax({
                url: '/Addresses/CreateAjax', // ✨ DOĞRU ENDPOINT
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(mappedData),
                dataType: 'json',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                }
            });

            $('#createAddressModal').modal('hide');
            this.showAlert(response.message || 'Address created!', 'success');
            this.loadAddresses();
            e.target.reset();
        } catch (error) {
            const msg = error.responseJSON?.message || error.message || "Bilinmeyen hata";
            this.showAlert('Error creating address: ' + msg, 'danger');
        }
    }


    // Edit address
    editAddress(id) {
        const address = this.filteredData.find(a => a.id === id);
        if (address) {
            this.populateEditForm(address);
            $('#editAddressModal').modal('show');
        }
    }

    // Populate edit form
    populateEditForm(address) {
        $('#EditId').val(address.id);
        $('#EditAccountId').val(address.accountId);
        $('#EditAddressName').val(address.addressName || '');
        $('#EditAddressType').val(address.addressType || '');
        $('#EditAddressLine1').val(address.addressLine1 || '');
        $('#EditAddressLine2').val(address.addressLine2 || '');
        $('#EditCity').val(address.city || '');
        $('#EditState').val(address.state || '');
        $('#EditCountry').val(address.country || '');
        $('#EditPhone').val(address.phone || '');
        $('#EditFax').val(address.fax || '');
        $('#EditIsDefault').prop('checked', address.isDefault);
        $('#EditActive').prop('checked', address.active);
    }

    // Update address
    async updateAddress(e) {
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const data = Object.fromEntries(formData.entries());
        
        // Map form field names to DTO property names
        const mappedData = {
            Id: data.Id,
            AccountId: data.AccountId,
            isDefault: data.isDefault === 'on',
            AddressType: data.AddressType,
            AddressName: data.AddressName,
            AddressLine1: data.AddressLine1,
            AddressLine2: data.AddressLine2,
            City: data.City,
            State: data.State,
            Country: data.Country,
            Phone: data.Phone,
            Fax: data.Fax,
            Active: data.Active === 'on'
        };
        
        // Get anti-forgery token
        const token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            this.showAlert('Security token not found. Please refresh the page.', 'danger');
            return;
        }
        
        console.log('Updating address with data:', mappedData);
        console.log('Token:', token);
        
        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/Update`,
                type: 'POST',
                data: JSON.stringify(mappedData),
                contentType: 'application/json',
                dataType: 'json',
                headers: {
                    'RequestVerificationToken': token
                }
            });

            console.log('Response:', response);
            $('#editAddressModal').modal('hide');
            this.showAlert('Address updated successfully!', 'success');
            this.loadAddresses();
        } catch (error) {
            console.error('Error updating address:', error);
            this.showAlert('Error updating address: ' + error.message, 'danger');
        }
    }

    // View address details
    async viewAddressDetails(id) {
        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/GetById/${id}`,
                type: 'GET',
                dataType: 'json'
            });
            
            this.populateDetailsModal(response);
            $('#addressDetailsModal').modal('show');
            $('#addressDetailsModal').data('address-id', id);
        } catch (error) {
            this.showAlert('Error loading address details: ' + error.message, 'danger');
        }
    }

    // Populate details modal
    populateDetailsModal(data) {
        $('#DetailsAddressName').text(data.addressName || 'N/A');
        $('#DetailsClientName').text(data.companyName || 'N/A');
        $('#DetailsAddressType').text(data.addressType || 'N/A');
        $('#DetailsAddressLine1').text(data.addressLine1 || 'N/A');
        $('#DetailsAddressLine2').text(data.addressLine2 || 'N/A');
        $('#DetailsCity').text(data.city || 'N/A');
        $('#DetailsState').text(data.state || 'N/A');
        $('#DetailsCountry').text(data.country || 'N/A');
        $('#DetailsPhone').text(data.phone || 'N/A');
        $('#DetailsFax').text(data.fax || 'N/A');
        $('#DetailsIsDefault').text(data.isDefault ? 'Yes' : 'No');
        $('#DetailsActive').html(this.getStatusBadge(data.active));
        $('#DetailsCreatedDate').text(data.createdDate ? 
            new Date(data.createdDate).toLocaleDateString('tr-TR') : 'N/A');
    }

    // Show delete confirmation
    showDeleteConfirmation(id) {
        $('#deleteConfirmModal').modal('show');
        $('#confirmDelete').off('click').on('click', () => this.deleteAddress(id));
    }

    // Delete address
    async deleteAddress(id) {
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
            this.showAlert('Address deleted successfully!', 'success');
            this.loadAddresses();
        } catch (error) {
            const msg = error.responseJSON?.message || error.message;
            this.showAlert('Error deleting address: ' + msg, 'danger');
        }
    }


    // Export addresses
    async exportAddresses() {
        try {
            this.showAlert('Exporting addresses...', 'info');
            // Implementation for exporting addresses
            // This would typically call an export endpoint
        } catch (error) {
            this.showAlert('Error exporting addresses: ' + error.message, 'danger');
        }
    }

    // Show loading spinner
    showLoading(show) {
        if (show) {
            $('#loadingSpinner').show();
            $('#addressesTable').hide();
        } else {
            $('#loadingSpinner').hide();
            $('#addressesTable').show();
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
    new AddressesManager();
}); 