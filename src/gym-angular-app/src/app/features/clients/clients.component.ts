import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientService } from '../../core/api-services/client.service';
import { ClientListItem, ClientDetails } from '../../core/models/client';
import { MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { BadgeModule } from 'primeng/badge';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    ToastModule,
    ToolbarModule,
    TagModule,
    TooltipModule,
    CardModule,
    BadgeModule
  ],
  providers: [MessageService],
  templateUrl: './clients.component.html',
  styleUrl: './clients.component.scss'
})
export class ClientsComponent implements OnInit {
  private clientService = inject(ClientService);
  private messageService = inject(MessageService);

  clients: ClientListItem[] = [];
  loading = false;
  searchQuery = '';
  
  detailsDialog = false;
  selectedClient: ClientDetails | null = null;
  loadingDetails = false;

  ngOnInit() {
    this.loadClients();
  }

  loadClients() {
    this.loading = true;
    this.clientService.getClients(this.searchQuery || undefined).subscribe({
      next: (clients: ClientListItem[]) => {
        this.clients = clients;
        this.loading = false;
      },
      error: (err: any) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load clients' });
        this.loading = false;
      }
    });
  }

  onSearch() {
    this.loadClients();
  }

  clearSearch() {
    this.searchQuery = '';
    this.loadClients();
  }

  viewClientDetails(client: ClientListItem) {
    this.loadingDetails = true;
    this.detailsDialog = true;
    this.clientService.getClientById(client.id).subscribe({
      next: (details: ClientDetails) => {
        this.selectedClient = details;
        this.loadingDetails = false;
      },
      error: (err: any) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load client details' });
        this.loadingDetails = false;
        this.detailsDialog = false;
      }
    });
  }

  hideDetailsDialog() {
    this.detailsDialog = false;
    this.selectedClient = null;
  }

  getStatusSeverity(status: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Expired':
        return 'warning';
      case 'Cancelled':
        return 'danger';
      case 'None':
      default:
        return 'secondary';
    }
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString('pl-PL');
  }
}
