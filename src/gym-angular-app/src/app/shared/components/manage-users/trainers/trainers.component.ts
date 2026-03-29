import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EmployeeService } from '../../../../core/api-services/employee.service';
import { Employee } from '../../../../core/models/employee';
import { MessageService } from 'primeng/api';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';

@Component({
  selector: 'app-trainers',
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
  templateUrl: './trainers.component.html',
  styleUrl: './trainers.component.scss'
})
export class TrainersComponent implements OnInit {
  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);

  trainers: Employee[] = [];
  loading = false;

  ngOnInit() {
    this.loadTrainers();
  }

  loadTrainers() {
    this.loading = true;
    this.employeeService.getAllEmployees().subscribe({
      next: (employees) => {
        this.trainers = employees.filter(e => e.role === 'Trainer');
        this.loading = false;
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load trainers' });
        this.loading = false;
      }
    });
  }

  getSeverity(role: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    return 'info';
  }
}
