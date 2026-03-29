import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ToastService } from '../../../core/services/toast.service';
import { EmployeeService } from '../../../core/api-services/employee.service';
import { Employment, EmployeeWithEmployments } from '../../../core/models/employment.model';

@Component({
  selector: 'app-employee-employments',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TooltipModule,
    CardModule,
    TagModule
  ],
  templateUrl: './employee-employments.component.html',
  styleUrl: './employee-employments.component.scss'
})
export class EmployeeEmploymentsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private employeeService = inject(EmployeeService);
  private toastService = inject(ToastService);

  employee = signal<EmployeeWithEmployments | null>(null);
  employments = signal<Employment[]>([]);
  loading = signal(false);

  ngOnInit() {
    const employeeId = Number(this.route.snapshot.params['id']);
    if (employeeId) {
      this.loadEmployments(employeeId);
    }
  }

  loadEmployments(employeeId: number) {
    this.loading.set(true);
    this.employeeService.getEmployeeEmployments(employeeId).subscribe({
      next: (data) => {
        this.employee.set(data);
        const employmentsWithStatus = data.employments.map(emp => ({
          ...emp,
          status: this.getEmploymentStatus(emp)
        }));
        this.employments.set(employmentsWithStatus);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.toastService.show('Failed to load employment records', 'error');
      }
    });
  }

  getEmploymentStatus(employment: Employment): 'Active' | 'Ended' {
    if (!employment.endDate) {
      return 'Active';
    }
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const endDate = new Date(employment.endDate);
    return endDate >= today ? 'Active' : 'Ended';
  }

  goBack() {
    const role = this.employee()?.role;
    if (role === 'Trainer') {
      this.router.navigate(['/management/manager/trainers']);
    } else {
      this.router.navigate(['/management/manager/receptionists']);
    }
  }

  formatDate(date: Date | undefined): string {
    if (!date) {
      return 'Present';
    }
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  getSeverity(status: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    return status === 'Active' ? 'success' : 'secondary';
  }
}
