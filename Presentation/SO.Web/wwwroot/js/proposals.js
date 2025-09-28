// Proposals Management System
class ProposalsManager {
    constructor() {
        this.baseUrl = '/Proposals';
        this.currentPage = 1;
        this.pageSize = 10;
        this.totalCount = 0;
        this.filteredData = [];
        this.initializeEventListeners();
        this.initializeFilters();
    }

    initializeEventListeners() {
        // Load proposals on page load
        if ($('#proposalsTable').length) {
            this.loadProposals();
        }

        // Search functionality
        $('#searchInput').on('input', this.debounce(() => this.applyFilters(), 300));
        $('#searchBtn').on('click', () => this.applyFilters());
        
        // Filter changes
        $('#statusFilter, #dateFilter').on('change', () => this.applyFilters());
        
        // Clear filters
        $('#clearFilters').on('click', () => this.clearFilters());
        
        // Refresh button
        $('#refreshBtn').on('click', () => this.loadProposals());
        
        // Export all button (rendered only if authorized)
        $('#exportAllBtn').on('click', () => {
            this.exportAllProposals();
        });
        
        // Pagination
        $('#prevPage').on('click', () => this.previousPage());
        $('#nextPage').on('click', () => this.nextPage());

        // Export to Word (button rendered only if authorized)
        $(document).on('click', '.export-proposal-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.exportToWord(id);
        });

        // View details buttons (works for all users)
        $(document).on('click', '.view-proposal-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.viewProposalDetails(id);
        });

        // Delete buttons (button rendered only if authorized)
        $(document).on('click', '.delete-proposal-btn', (e) => {
            e.preventDefault();
            const id = $(e.target).data('id');
            this.showDeleteConfirmation(id);
        });

        // Edit from modal
        $('#editFromModal').on('click', () => {
            const id = $('#proposalDetailsModal').data('proposal-id');
            if (id) {
                window.location.href = `${this.baseUrl}/Edit/${id}`;
            }
        });
    }

    initializeFilters() {
        // Set default values
        $('#statusFilter').val('');
        $('#dateFilter').val('');
        $('#searchInput').val('');
    }

    // Load all proposals
    async loadProposals() {
        try {
            this.showLoading(true);
            const response = await $.ajax({
                url: `${this.baseUrl}/GetAllProposals`,
                type: 'GET',
                dataType: 'json'
            }).catch(xhr => this.handleAjaxError(xhr));
            
            this.filteredData = response || [];
            this.totalCount = this.filteredData.length;
            this.currentPage = 1;
            this.applyFilters();
        } catch (error) {
            this.showAlert('Error loading proposals: ' + error.message, 'danger');
        } finally {
            this.showLoading(false);
        }
    }

    // Apply search and filters
    applyFilters() {
        const searchTerm = $('#searchInput').val().toLowerCase();
        const statusFilter = $('#statusFilter').val();
        const dateFilter = $('#dateFilter').val();

        let filtered = this.filteredData.filter(proposal => {
            // Search filter
            const matchesSearch = !searchTerm || 
                proposal.proposalName.toLowerCase().includes(searchTerm) ||
                proposal.companyName.toLowerCase().includes(searchTerm);

            // Status filter
            const matchesStatus = !statusFilter || proposal.status === statusFilter;

            // Date filter
            const matchesDate = !dateFilter || this.matchesDateFilter(proposal.proposalDate, dateFilter);

            return matchesSearch && matchesStatus && matchesDate;
        });

        this.displayProposals(filtered);
        this.updatePagination(filtered.length);
    }

    // Check if proposal date matches filter
    matchesDateFilter(proposalDate, filter) {
        const date = new Date(proposalDate);
        const now = new Date();
        
        switch (filter) {
            case 'today':
                return this.isSameDay(date, now);
            case 'week':
                const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
                return date >= weekAgo;
            case 'month':
                const monthAgo = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
                return date >= monthAgo;
            case 'quarter':
                const quarterAgo = new Date(now.getTime() - 90 * 24 * 60 * 60 * 1000);
                return date >= quarterAgo;
            case 'year':
                const yearAgo = new Date(now.getTime() - 365 * 24 * 60 * 60 * 1000);
                return date >= yearAgo;
            default:
                return true;
        }
    }

    isSameDay(date1, date2) {
        return date1.getFullYear() === date2.getFullYear() &&
               date1.getMonth() === date2.getMonth() &&
               date1.getDate() === date2.getDate();
    }

    // Clear all filters
    clearFilters() {
        $('#searchInput').val('');
        $('#statusFilter').val('');
        $('#dateFilter').val('');
        this.applyFilters();
    }

    // Display proposals in table
    displayProposals(proposals) {
        const tbody = $('#proposalsTable tbody');
        tbody.empty();

        if (proposals && proposals.length > 0) {
            const startIndex = (this.currentPage - 1) * this.pageSize;
            const endIndex = startIndex + this.pageSize;
            const pageProposals = proposals.slice(startIndex, endIndex);

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
                                <i class="fas fa-building text-secondary me-2"></i>
                                ${this.escapeHtml(proposal.companyName)}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-calendar text-info me-2"></i>
                                ${new Date(proposal.proposalDate).toLocaleDateString('tr-TR')}
                            </div>
                        </td>
                        <td>
                            <div class="d-flex align-items-center">
                                <i class="fas fa-money-bill-wave text-success me-2"></i>
                                <strong>${proposal.totalAmount.toLocaleString('tr-TR', { 
                                    style: 'currency', 
                                    currency: proposal.currency || 'TRY' 
                                })}</strong>
                            </div>
                        </td>
                        <td>${statusBadge}</td>
                        <td class="text-center">
                            <div class="action-buttons">
                                ${this.isAuthenticated ? `
                                    <a href="${this.baseUrl}/Edit/${proposal.id}" class="btn btn-outline-primary btn-sm" 
                                       title="Edit Proposal">
                                        <i class="fas fa-edit"></i>
                                    </a>
                                    <button class="btn btn-outline-success btn-sm export-proposal-btn" 
                                            data-id="${proposal.id}" title="Export to Word">
                                        <i class="fas fa-file-word"></i>
                                    </button>
                                    <button class="btn btn-outline-danger btn-sm delete-proposal-btn" 
                                            data-id="${proposal.id}" title="Delete Proposal">
                                        <i class="fas fa-trash"></i>
                                    </button>
                                ` : `
                                    <button class="btn btn-outline-info btn-sm view-proposal-btn" 
                                            data-id="${proposal.id}" title="View Details">
                                        <i class="fas fa-eye"></i>
                                    </button>
                                    <div class="mt-1">
                                        <small class="text-muted">
                                            <i class="fas fa-lock me-1"></i>Login required
                                        </small>
                                    </div>
                                `}
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
                            <p class="text-muted mb-0">No proposals found matching your criteria.</p>
                        </div>
                    </td>
                </tr>
            `);
        }

        this.updateCounts(proposals.length);
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
    getStatusBadge(status) {
        const statusClass = this.getStatusClass(status);
        const statusText = status.charAt(0).toUpperCase() + status.slice(1);
        
        return `<span class="status-badge ${statusClass}">${statusText}</span>`;
    }

    // Get status CSS class
    getStatusClass(status) {
        switch (status.toLowerCase()) {
            case 'draft': return 'status-draft';
            case 'sent': return 'status-sent';
            case 'approved': return 'status-approved';
            case 'rejected': return 'status-rejected';
            case 'cancelled': return 'status-cancelled';
            default: return 'status-draft';
        }
    }

    // View proposal details
    async viewProposalDetails(id) {
        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/GetById/${id}`,
                type: 'GET',
                dataType: 'json'
            });
            
            this.populateDetailsModal(response);
            $('#proposalDetailsModal').modal('show');
            $('#proposalDetailsModal').data('proposal-id', id);
        } catch (error) {
            this.showAlert('Error loading proposal details: ' + error.message, 'danger');
        }
    }

    // Populate details modal
    populateDetailsModal(data) {
        $('#DetailsProposalName').text(data.proposalName || 'N/A');
        $('#DetailsCompanyName').text(data.companyName || 'N/A');
        $('#DetailsPreparedBy').text(data.preparedBy || 'N/A');
        $('#DetailsProposalDate').text(new Date(data.proposalDate).toLocaleDateString('tr-TR') || 'N/A');
        $('#DetailsStatus').html(this.getStatusBadge(data.status));
        $('#DetailsTotalAmount').text(data.totalAmount ? 
            data.totalAmount.toLocaleString('tr-TR', { style: 'currency', currency: data.currency || 'TRY' }) : 'N/A');
        $('#DetailsProjectDescription').text(data.projectDescription || 'N/A');
        $('#DetailsStatementOfNeed').text(data.statementOfNeed || 'N/A');
    }

    // Show delete confirmation
    showDeleteConfirmation(id) {
        $('#deleteConfirmModal').modal('show');
        $('#confirmDelete').off('click').on('click', () => this.deleteProposal(id));
    }

    // Delete proposal
    async deleteProposal(id) {
        try {
            await $.ajax({
                url: `${this.baseUrl}/Delete/${id}`,
                type: 'POST',
                data: { id: id },
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                }
            }).catch(xhr => this.handleAjaxError(xhr));

            $('#deleteConfirmModal').modal('hide');
            this.showAlert('Proposal deleted successfully!', 'success');
            this.loadProposals();
        } catch (error) {
            this.showAlert('Error deleting proposal: ' + error.message, 'danger');
        }
    }

    // Export to Word
    async exportToWord(id) {
        try {
            const response = await $.ajax({
                url: `${this.baseUrl}/ExportToWord/${id}`,
                type: 'GET',
                xhrFields: {
                    responseType: 'blob'
                }
            }).catch(xhr => this.handleAjaxError(xhr));

            // Create download link
            const blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `proposal-${id}.docx`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);

            this.showAlert('Proposal exported successfully!', 'success');
        } catch (error) {
            this.showAlert('Error exporting proposal: ' + error.message, 'danger');
        }
    }

    // Centralized AJAX error handler
    handleAjaxError(xhr) {
        if (!xhr) return;
        if (xhr.status === 403) {
            this.showAlert('You do not have permission to perform this action.', 'warning');
        } else if (xhr.status === 401) {
            this.showAlert('Please login to continue.', 'info');
        } else {
            const msg = (xhr.responseJSON && xhr.responseJSON.error) ? xhr.responseJSON.error : 'Unexpected error occurred.';
            this.showAlert(msg, 'danger');
        }
    }

    // Export all proposals
    async exportAllProposals() {
        try {
            this.showAlert('Exporting all proposals...', 'info');
            // Implementation for exporting all proposals
            // This would typically call a bulk export endpoint
        } catch (error) {
            this.showAlert('Error exporting proposals: ' + error.message, 'danger');
        }
    }

    // Show loading spinner
    showLoading(show) {
        if (show) {
            $('#loadingSpinner').show();
            $('#proposalsTable').hide();
        } else {
            $('#loadingSpinner').hide();
            $('#proposalsTable').show();
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
    new ProposalsManager();
}); 