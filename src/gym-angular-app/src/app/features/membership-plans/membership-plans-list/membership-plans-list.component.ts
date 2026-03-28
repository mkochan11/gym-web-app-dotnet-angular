import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MembershipPlanService } from '../../../core/api-services/membership-plan.service';
import { AuthService } from '../../../core/api-services/auth.service';
import { MembershipPlan, CreateMembershipPlanRequest, UpdateMembershipPlanRequest } from '../../../core/models/membership-plan.model';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputSwitchModule } from 'primeng/inputswitch';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-membership-plans-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    ToastModule,
    ToolbarModule,
    TagModule,
    TooltipModule,
    ConfirmDialogModule,
    InputTextareaModule,
    InputNumberModule,
    InputSwitchModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './membership-plans-list.component.html',
  styleUrl: './membership-plans-list.component.scss'
})
export class MembershipPlansListComponent implements OnInit {
  private membershipPlanService = inject(MembershipPlanService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);

  plans: MembershipPlan[] = [];
  loading = false;
  submitted = false;
  isEditMode = false;
  editingPlanId: number | null = null;

  planDialog = false;
  planForm: FormGroup;

  deleteDialog = false;
  planToDelete: MembershipPlan | null = null;

  get canEditPlans(): boolean {
    const role = this.authService.getRole();
    if (!role) return false;
    const roles = role.split(',').map(r => r.trim().toLowerCase());
    return roles.includes('admin') || roles.includes('manager');
  }

  constructor() {
    this.planForm = this.fb.group({
      type: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(1000)],
      price: [null, [Validators.required, Validators.min(0.01)]],
      durationTime: ['', [Validators.required, Validators.maxLength(50)]],
      durationInMonths: [null, [Validators.required, Validators.min(1)]],
      canReserveTrainings: [false],
      canAccessGroupTraining: [false],
      canAccessPersonalTraining: [false],
      canReceiveTrainingPlans: [false],
      maxTrainingsPerMonth: [null],
      isActive: [true]
    });
  }

  ngOnInit() {
    console.log('[MembershipPlans] Component initialized');
    this.loadPlans();
  }

  loadPlans() {
    console.log('[MembershipPlans] Loading plans...');
    this.loading = true;
    this.membershipPlanService.getMembershipPlans().subscribe({
      next: (plans) => {
        console.log('[MembershipPlans] Plans loaded:', plans.length);
        this.plans = plans;
        this.loading = false;
      },
      error: (err) => {
        console.log('[MembershipPlans] Error loading plans:', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load membership plans' });
        this.loading = false;
      }
    });
  }

  hideDialog() {
    this.planDialog = false;
    this.submitted = false;
    this.isEditMode = false;
    this.editingPlanId = null;
  }

  openNewPlan() {
    this.planForm.reset({
      canReserveTrainings: false,
      canAccessGroupTraining: false,
      canAccessPersonalTraining: false,
      canReceiveTrainingPlans: false,
      maxTrainingsPerMonth: null,
      isActive: true
    });
    this.submitted = false;
    this.isEditMode = false;
    this.editingPlanId = null;
    this.planDialog = true;
  }

  editPlan(plan: MembershipPlan) {
    this.isEditMode = true;
    this.editingPlanId = plan.id;
    this.planForm.patchValue({
      type: plan.type,
      description: plan.description,
      price: plan.price,
      durationTime: plan.durationTime,
      durationInMonths: plan.durationInMonths,
      canReserveTrainings: plan.canReserveTrainings,
      canAccessGroupTraining: plan.canAccessGroupTraining,
      canAccessPersonalTraining: plan.canAccessPersonalTraining,
      canReceiveTrainingPlans: plan.canReceiveTrainingPlans,
      maxTrainingsPerMonth: plan.maxTrainingsPerMonth,
      isActive: plan.isActive
    });
    this.submitted = false;
    this.planDialog = true;
  }

  savePlan() {
    this.submitted = true;

    if (this.planForm.invalid) {
      return;
    }

    if (this.isEditMode && this.editingPlanId) {
      this.updatePlan();
    } else {
      this.createPlan();
    }
  }

  private createPlan() {
    const planData: CreateMembershipPlanRequest = this.planForm.value;

    this.membershipPlanService.createMembershipPlan(planData).subscribe({
      next: (newPlan) => {
        this.plans.push(newPlan);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Membership plan created successfully' });
        this.hideDialog();
      },
      error: (err) => {
        const errorMessage = err.message || 'Failed to create membership plan';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
      }
    });
  }

  private updatePlan() {
    if (!this.editingPlanId) return;

    const planData: UpdateMembershipPlanRequest = {
      id: this.editingPlanId,
      ...this.planForm.value
    };

    this.membershipPlanService.updateMembershipPlan(this.editingPlanId, planData).subscribe({
      next: (updatedPlan) => {
        const index = this.plans.findIndex(p => p.id === updatedPlan.id);
        if (index !== -1) {
          this.plans[index] = updatedPlan;
        }
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Membership plan updated successfully' });
        this.hideDialog();
      },
      error: (err) => {
        const errorMessage = err.message || 'Failed to update membership plan';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
      }
    });
  }

  confirmDeletePlan(plan: MembershipPlan): void {
    this.planToDelete = plan;
    this.deleteDialog = true;
  }

  hideDeleteDialog(): void {
    this.deleteDialog = false;
    this.planToDelete = null;
  }

  deletePlan(): void {
    if (!this.planToDelete) return;

    this.membershipPlanService.deleteMembershipPlan(this.planToDelete.id).subscribe({
      next: () => {
        this.plans = this.plans.filter(p => p.id !== this.planToDelete!.id);
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Membership plan deleted successfully' });
        this.hideDeleteDialog();
      },
      error: (err) => {
        const errorMessage = err.message || 'Failed to delete membership plan';
        this.messageService.add({ severity: 'error', summary: 'Error', detail: errorMessage });
        this.hideDeleteDialog();
      }
    });
  }

  getSeverityStatus(isActive: boolean): 'success' | 'danger' | undefined {
    return isActive ? 'success' : 'danger';
  }

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  formatFeatures(plan: MembershipPlan): string[] {
    const features: string[] = [];
    if (plan.canReserveTrainings) features.push('Reserve Trainings');
    if (plan.canAccessGroupTraining) features.push('Group Training');
    if (plan.canAccessPersonalTraining) features.push('Personal Training');
    if (plan.canReceiveTrainingPlans) features.push('Training Plans');
    if (plan.maxTrainingsPerMonth) features.push(`${plan.maxTrainingsPerMonth} trainings/mo`);
    return features;
  }
}
