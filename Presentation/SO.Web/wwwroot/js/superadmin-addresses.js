class SuperAdminAddressesManager {
    constructor() {
        this.allAddresses = [];
        this.filteredAddresses = [];
        this.currentPage = 1;
        this.itemsPerPage = 10;
        this.currentCityFilter = 'all';
        this.currentUserFilter = 'all';
        this.searchTerm = '';
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.loadAddresses();
    }

    bindEvents() {
        $('#searchInput').on('input', (e) => {
            this.searchTerm = e.target.value.toLowerCase();
            this.applyFilters();
        });

        $('#cityFilter').on('change', (e) => {
            this.currentCityFilter = e.target.value;
            this.applyFilters();
        });

        $('#userFilter').on('change', (e) => {
            this.currentUserFilter = e.target.value;
            this.applyFilters();
        });
    }

    async loadAddresses() {
        try {
            console.log('Loading all addresses for SuperAdmin...');
            const response = await fetch('/SuperAdmin/GetAllAddresses');
            console.log('Response status:', response.status);
            
            if (response.ok) {
                const data = await response.json();
                console.log('API Response:', data);
                
                if (data && data.length > 0) {
                    this.allAddresses = data.map(address => ({
                        id: address.id,
                        addressLine: address.addressLine || 'No Address',
                        city: address.city || 'No City',
                        country: address.country || 'No Country',
                        postalCode: address.postalCode || 'No Postal Code',
                        companyName: address.companyName || 'No Company',
                        createdBy: address.createdBy || 'Unknown',
                        createdDate: new Date(address.createdDate)
                    }));
                    
                    this.filteredAddresses = [...this.allAddresses];
                    this.populateCityFilter();
                    this.populateUserFilter();
                    this.displayAddresses();
                    this.updatePagination();
                    console.log('Loaded addresses:', this.allAddresses.length);
                } else {
                    this.showAlert('No addresses found', 'info');
                    this.allAddresses = [];
                    this.filteredAddresses = [];
                    this.displayAddresses();
                }
            } else {
                const errorText = await response.text();
                console.error('API Error:', errorText);
                this.showAlert('Error loading addresses', 'danger');
            }
        } catch (error) {
            console.error('Error loading addresses:', error);
            this.showAlert('Error loading addresses', 'danger');
        }
    }

    populateCityFilter() {
        const cityFilter = $('#cityFilter');
        const uniqueCities = [...new Set(this.allAddresses.map(a => a.city))];
        
        cityFilter.empty();
        cityFilter.append('<option value="all">All Cities</option>');
        
        uniqueCities.forEach(city => {
            cityFilter.append(`<option value="${city}">${city}</option>`);
        });
    }

    populateUserFilter() {
        const userFilter = $('#userFilter');
        const uniqueUsers = [...new Set(this.allAddresses.map(a => a.createdBy))];
        
        userFilter.empty();
        userFilter.append('<option value="all">All Users</option>');
        
        uniqueUsers.forEach(user => {
            userFilter.append(`<option value="${user}">${user}</option>`);
        });
    }

    applyFilters() {
        this.filteredAddresses = this.allAddresses.filter(address => {
            const matchesSearch = address.addressLine.toLowerCase().includes(this.searchTerm) ||
                                address.city.toLowerCase().includes(this.searchTerm) ||
                                address.companyName.toLowerCase().includes(this.searchTerm);
            const matchesCity = this.currentCityFilter === 'all' || address.city === this.currentCityFilter;
            const matchesUser = this.currentUserFilter === 'all' || address.createdBy === this.currentUserFilter;
            
            return matchesSearch && matchesCity && matchesUser;
        });
        
        this.currentPage = 1;
        this.displayAddresses();
        this.updatePagination();
    }

    displayAddresses() {
        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        const endIndex = startIndex + this.itemsPerPage;
        const addressesToShow = this.filteredAddresses.slice(startIndex, endIndex);
        
        const tbody = $('#addressesTableBody');
        tbody.empty();
        
        if (addressesToShow.length === 0) {
            tbody.append(`
                <tr>
                    <td colspan="8" class="text-center py-4">
                        <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                        <p class="text-muted mb-0">No addresses found</p>
                    </td>
                </tr>
            `);
            return;
        }
        
        addressesToShow.forEach(address => {
            const row = `
                <tr>
                    <td>${address.addressLine}</td>
                    <td>${address.city}</td>
                    <td>${address.country}</td>
                    <td>${address.postalCode}</td>
                    <td>${address.companyName}</td>
                    <td>${address.createdBy}</td>
                    <td>${address.createdDate.toLocaleDateString()}</td>
                    <td>
                        <div class="btn-group" role="group">
                            <a href="/Addresses/Edit/${address.id}" class="btn btn-sm btn-primary" title="Edit" onclick="console.log('Edit clicked for address:', '${address.id}')">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a href="/Addresses/Details/${address.id}" class="btn btn-sm btn-info" title="View Details" onclick="console.log('Details clicked for address:', '${address.id}')">
                                <i class="fas fa-eye"></i>
                            </a>
                            <button class="btn btn-sm btn-danger" title="Delete" onclick="superAdminAddressesManager.deleteAddress('${address.id}')">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            `;
            tbody.append(row);
        });
        
        // Update count
        $('#addressCount').text(`${this.filteredAddresses.length} addresses`);
    }

    updatePagination() {
        const totalPages = Math.ceil(this.filteredAddresses.length / this.itemsPerPage);
        const pagination = $('#pagination');
        pagination.empty();
        
        if (totalPages <= 1) return;
        
        // Previous button
        pagination.append(`
            <li class="page-item ${this.currentPage === 1 ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="superAdminAddressesManager.goToPage(${this.currentPage - 1})">Previous</a>
            </li>
        `);
        
        // Page numbers
        for (let i = 1; i <= totalPages; i++) {
            pagination.append(`
                <li class="page-item ${i === this.currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" onclick="superAdminAddressesManager.goToPage(${i})">${i}</a>
                </li>
            `);
        }
        
        // Next button
        pagination.append(`
            <li class="page-item ${this.currentPage === totalPages ? 'disabled' : ''}">
                <a class="page-link" href="#" onclick="superAdminAddressesManager.goToPage(${this.currentPage + 1})">Next</a>
            </li>
        `);
    }

    goToPage(page) {
        const totalPages = Math.ceil(this.filteredAddresses.length / this.itemsPerPage);
        if (page >= 1 && page <= totalPages) {
            this.currentPage = page;
            this.displayAddresses();
            this.updatePagination();
        }
    }

    async deleteAddress(addressId) {
        if (!confirm('Are you sure you want to delete this address?')) {
            return;
        }
        
        try {
            const token = $('input[name="__RequestVerificationToken"]').val();
            
            // FormData kullanarak POST request gönder
            const formData = new FormData();
            formData.append('id', addressId);
            formData.append('__RequestVerificationToken', token);
            
            const response = await fetch('/Addresses/Delete', {
                method: 'POST',
                body: formData
            });
            
            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    this.showAlert('Address deleted successfully', 'success');
                    this.loadAddresses();
                } else {
                    this.showAlert(result.message || 'Error deleting address', 'danger');
                }
            } else {
                const errorText = await response.text();
                console.error('Delete error:', errorText);
                this.showAlert('Error deleting address: ' + errorText, 'danger');
            }
        } catch (error) {
            console.error('Error deleting address:', error);
            this.showAlert('Error deleting address: ' + error.message, 'danger');
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
    window.superAdminAddressesManager = new SuperAdminAddressesManager();
});
