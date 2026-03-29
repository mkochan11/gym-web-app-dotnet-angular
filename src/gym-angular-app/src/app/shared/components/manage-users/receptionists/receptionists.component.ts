import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { UserService } from '../../../../core/api-services/user.service';
import { User } from '../../../../core/models/user';
import { MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';

@Component({
  selector: 'app-receptionists',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TableModule,
    ButtonModule,
    TagModule,
    TooltipModule,
    ToastModule,
    ToolbarModule
  ],
  providers: [MessageService],
  templateUrl: './receptionists.component.html',
  styleUrl: './receptionists.component.scss'
})
export class ReceptionistsComponent implements OnInit {
  private userService = inject(UserService);
  private messageService = inject(MessageService);

  receptionists: User[] = [];
  loading = false;

  ngOnInit() {
    this.loadReceptionists();
  }

  loadReceptionists() {
    this.loading = true;
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.receptionists = users.filter(u => u.role === 'Receptionist');
        this.loading = false;
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load receptionists' });
        this.loading = false;
      }
    });
  }

  getSeverity(role: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    return 'secondary';
  }
}
