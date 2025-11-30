import { Component, Input, Output, EventEmitter, OnInit, inject, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { EmployeeService, TrainingTypeService } from '../../../../core/api-services';
import { Employee } from '../../../../core/models/employee';
import { TrainingType } from '../../../../core/models/training-type';

interface EventTypeOption {
  value: 'group' | 'individual' | 'shift';
  label: string;
  description: string;
  color: string;
  icon: string;
}

@Component({
  selector: 'app-manager-calendar-event-add-modal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    CalendarModule,
    DropdownModule
  ],
  template: `
    <div class="modal-overlay" [class.show]="visible" (click)="onOverlayClick($event)">
      <div class="modal-content" (click)="$event.stopPropagation()">
        
        <div class="stepper-header">
          <div class="steps">
            <div class="step" [class.active]="currentStep === 1" [class.completed]="currentStep > 1">
              <div class="step-circle">
                <span class="step-number">1</span>
                <i class="pi pi-check step-complete-icon" *ngIf="currentStep > 1"></i>
              </div>
              <span class="step-label">Event Type</span>
            </div>
            
            <div class="step-connector" [class.active]="currentStep > 1"></div>
            
            <div class="step" [class.active]="currentStep === 2" [class.completed]="currentStep > 2">
              <div class="step-circle">
                <span class="step-number">2</span>
                <i class="pi pi-check step-complete-icon" *ngIf="currentStep > 2"></i>
              </div>
              <span class="step-label">Event Details</span>
            </div>
          </div>
          
          <button class="close-btn" (click)="onCancel()">
            <i class="pi pi-times"></i>
          </button>
        </div>

        <div class="step-content" *ngIf="currentStep === 1">
          <div class="step-title">
            <h3>What type of event would you like to create?</h3>
            <p>Select the type of event that best fits your needs</p>
          </div>
          
          <div class="event-type-grid">
            <div 
              *ngFor="let type of availableEventTypes" 
              class="event-type-card"
              [class.selected]="selectedEventType === type.value"
              (click)="selectEventType(type)">
              
              <div class="type-icon" [style.background]="type.color">
                <i [class]="type.icon"></i>
              </div>
              
              <div class="type-content">
                <h4>{{ type.label }}</h4>
                <p>{{ type.description }}</p>
              </div>
              
              <div class="type-check">
                <i class="pi pi-check" *ngIf="selectedEventType === type.value"></i>
              </div>
            </div>
          </div>

          <div *ngIf="availableEventTypes.length === 0" class="no-types-message">
            <i class="pi pi-info-circle"></i>
            <p>You don't have permission to create any events.</p>
          </div>
        </div>

        <div class="step-content" *ngIf="currentStep === 2">
          <div class="step-title">
            <h3>Create {{ getEventTypeLabel() }}</h3>
            <p>Fill in the details for your new event</p>
          </div>

          <form #eventForm="ngForm" class="event-form">
            <div class="form-section">
              <h4>Date & Time</h4>
              <div class="form-row">
                <div class="form-group">
                  <label for="startTime">Start Time *</label>
                  <p-calendar
                    id="startTime"
                    [(ngModel)]="formData.startTime"
                    name="startTime"
                    [showTime]="true"
                    [showIcon]="true"
                    [required]="true"
                    [minDate]="minDate"
                    [timeOnly]="false"
                    (onSelect)="onTimeChange()"
                    [class]="{'invalid': submitted && !formData.startTime}">
                  </p-calendar>
                  <small class="error-message" *ngIf="submitted && !formData.startTime">
                    Start time is required
                  </small>
                </div>

                <div class="form-group">
                  <label for="endTime">End Time *</label>
                  <p-calendar
                    id="endTime"
                    [(ngModel)]="formData.endTime"
                    name="endTime"
                    [showTime]="true"
                    [showIcon]="true"
                    [required]="true"
                    [minDate]="minDate"
                    [timeOnly]="false"
                    (onSelect)="onTimeChange()"
                    [class]="{'invalid': submitted && !formData.endTime}">
                  </p-calendar>
                  <small class="error-message" *ngIf="submitted && !formData.endTime">
                    End time is required
                  </small>
                </div>
              </div>
            </div>

            <!-- Event Specific Fields -->
            <div class="form-section" *ngIf="selectedEventType">
              <h4>{{ getEventTypeLabel() }} Details</h4>
              
              <!-- Group Training Fields -->
              <div *ngIf="selectedEventType === 'group'" class="form-row">
                <div class="form-group">
                  <label for="trainingType">Training Type *</label>
                  <p-dropdown
                    id="trainingType"
                    [(ngModel)]="formData.trainingTypeId"
                    [options]="trainingTypeOptions"
                    name="trainingType"
                    optionLabel="name"
                    optionValue="id"
                    [required]="true"
                    placeholder="Select training type"
                    [class]="{'invalid': submitted && !formData.trainingTypeId}">
                  </p-dropdown>
                  <small class="error-message" *ngIf="submitted && !formData.trainingTypeId">
                    Training type is required
                  </small>
                </div>

                <div class="form-group">
                  <label for="trainer">Trainer *</label>
                  <p-dropdown
                    id="trainer"
                    [(ngModel)]="formData.trainerId"
                    [options]="trainerOptions"
                    name="trainer"
                    optionLabel="name"
                    optionValue="id"
                    [required]="true"
                    placeholder="Select trainer"
                    [class]="{'invalid': submitted && !formData.trainerId}">
                  </p-dropdown>
                  <small class="error-message" *ngIf="submitted && !formData.trainerId">
                    Trainer is required
                  </small>
                </div>
              </div>

              <!-- Individual Training Fields -->
              <div *ngIf="selectedEventType === 'individual'" class="form-group">
                <label for="trainer">Trainer *</label>
                <p-dropdown
                  id="trainer"
                  [(ngModel)]="formData.trainerId"
                  [options]="trainerOptions"
                  name="trainer"
                  optionLabel="name"
                  optionValue="id"
                  [required]="true"
                  placeholder="Select trainer"
                  [class]="{'invalid': submitted && !formData.trainerId}">
                </p-dropdown>
                <small class="error-message" *ngIf="submitted && !formData.trainerId">
                  Trainer is required
                </small>
              </div>

              <!-- Shift Fields -->
              <div *ngIf="selectedEventType === 'shift'" class="form-group">
                <label for="employee">Employee *</label>
                <p-dropdown
                  id="employee"
                  [(ngModel)]="formData.employeeId"
                  [options]="receptionistOptions"
                  name="employee"
                  optionLabel="name"
                  optionValue="id"
                  [required]="true"
                  placeholder="Select employee"
                  [class]="{'invalid': submitted && !formData.employeeId}">
                </p-dropdown>
                <small class="error-message" *ngIf="submitted && !formData.employeeId">
                  Employee is required
                </small>
              </div>

              <!-- Additional Fields -->
              <div class="form-row" *ngIf="selectedEventType === 'group'">
                <div class="form-group">
                  <label for="maxParticipants">Max Participants</label>
                  <input
                    id="maxParticipants"
                    type="number"
                    pInputText
                    [(ngModel)]="formData.maxParticipants"
                    name="maxParticipants"
                    min="1"
                    max="50">
                </div>

                <div class="form-group">
                  <label for="difficultyLevel">Difficulty Level</label>
                  <p-dropdown
                    id="difficultyLevel"
                    [(ngModel)]="formData.difficultyLevel"
                    [options]="difficultyLevels"
                    name="difficultyLevel"
                    placeholder="Select difficulty level">
                  </p-dropdown>
                </div>
              </div>
            </div>

            <div class="form-section" *ngIf="selectedEventType === 'group' || selectedEventType === 'individual'">
              <div class="form-group">
                <label for="description">Description</label>
                <textarea
                  id="description"
                  pInputTextarea
                  [(ngModel)]="formData.description"
                  name="description"
                  rows="3"
                  placeholder="Enter event description...">
                </textarea>
              </div>
            </div>
          </form>
        </div>

        <div class="modal-footer">
          <button 
            pButton 
            label="Cancel" 
            class="p-button-text" 
            (click)="onCancel()">
          </button>
          
          <div class="action-buttons">
            <button 
              *ngIf="currentStep > 1"
              pButton 
              label="Back" 
              class="p-button-secondary" 
              (click)="previousStep()">
            </button>
            
            <button 
              *ngIf="currentStep === 1"
              pButton 
              label="Continue" 
              class="p-button-primary" 
              (click)="nextStep()"
              [disabled]="!selectedEventType">
            </button>
            
            <button 
              *ngIf="currentStep === 2"
              pButton 
              label="Create Event" 
              class="p-button-primary" 
              (click)="onSubmit()"
              [loading]="loading">
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./manager-calendar-event-add.component.scss']
})
export class ManagerCalendarEventAddModalComponent implements OnInit, OnChanges {
  @Input() visible = false;
  @Input() startTime!: Date;
  @Input() endTime!: Date;
  
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() eventCreated = new EventEmitter<any>();
  @Output() cancelled = new EventEmitter<void>();

  private employeeService = inject(EmployeeService);
  private trainingTypeService = inject(TrainingTypeService);

  currentStep = 1;
  selectedEventType: 'group' | 'individual' | 'shift' | null = null;
  formData: any = {};
  submitted = false;
  loading = false;

  minDate: Date = new Date();
  timeConstraints = {
    hourMin: 8,
    hourMax: 22
  };

  trainerOptions: any[] = [];
  receptionistOptions: any[] = [];
  trainingTypeOptions: any[] = [];
  difficultyLevels = [
    { label: 'Level 1 - Beginner', value: 1 },
    { label: 'Level 2 - Basic', value: 2 },
    { label: 'Level 3 - Intermediate', value: 3 },
    { label: 'Level 4 - Advanced', value: 4 },
    { label: 'Level 5 - Expert', value: 5 }
  ];

  availableEventTypes: EventTypeOption[] = [
      {
        value: 'group',
        label: 'Group Training',
        description: 'Training session for multiple participants',
        color: '#10B981',
        icon: 'pi pi-users'
      },
      {
        value: 'individual',
        label: 'Individual Training',
        description: 'One-on-one personal training session',
        color: '#3B82F6',
        icon: 'pi pi-user'
      },
      {
        value: 'shift',
        label: 'Staff Shift',
        description: 'Employee work schedule',
        color: '#F59E0B',
        icon: 'pi pi-briefcase'
      }
    ];

  ngOnChanges(changes: SimpleChanges) {
    if ((changes['visible'] && this.visible) || 
        ((changes['startTime'] || changes['endTime']) && this.visible)) {
      this.initializeFormData();
      this.setMinDate();
    }
  }

  ngOnInit() {
    this.loadDropdownData();
    this.setMinDate();
  }

  private setMinDate() {
    this.minDate = new Date();
    this.minDate.setMinutes(0, 0, 0);
  }

  private initializeFormData() {
    const start = this.startTime && this.isValidDate(this.startTime) 
      ? new Date(this.startTime) 
      : new Date();
    
    const end = this.endTime && this.isValidDate(this.endTime)
      ? new Date(this.endTime)
      : new Date(start.getTime() + 60 * 60 * 1000);

    this.formData = {
      startTime: start,
      endTime: end,
      description: '',
      maxParticipants: 10
    };
  }

  private isValidDate(date: any): boolean {
    return date instanceof Date && !isNaN(date.getTime());
  }

  private loadDropdownData() {
    this.employeeService.getAllEmployees().subscribe({
      next: (employees: Employee[]) => {
        this.receptionistOptions = employees
        .filter(emp => emp.role === 'Receptionist')
        .map(emp => ({
          id: emp.id,
          name: `${emp.firstName} ${emp.lastName}`
        }));

        this.trainerOptions = employees
        .filter(emp => emp.role === 'Trainer')
        .map(emp => ({
          id: emp.id,
          name: `${emp.firstName} ${emp.lastName}`
        }));
      }
    });

    this.trainingTypeService.getAllTrainingTypes().subscribe({
      next: (trainingTypes: TrainingType[]) => {
        this.trainingTypeOptions = trainingTypes.map(tt => ({
          id: tt.id,
          name: tt.name
        }));
      }
    });
  }

  selectEventType(type: EventTypeOption) {
    this.selectedEventType = type.value;
  }

  nextStep() {
    if (this.currentStep === 1 && this.selectedEventType) {
      this.currentStep = 2;
    }
  }

  previousStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  onSubmit() {
    this.submitted = true;

    if (!this.isFormValid()) {
      return;
    }

    this.loading = true;

    setTimeout(() => {
      const result = {
        type: this.selectedEventType,
        startTime: this.formData.startTime,
        endTime: this.formData.endTime,
        formData: { ...this.formData }
      };
      
      this.eventCreated.emit(result);
      this.loading = false;
      this.closeModal();
    }, 100);
  }

  private isFormValid(): boolean {
    if (!this.formData.startTime || !this.formData.endTime) {
      return false;
    }

    switch (this.selectedEventType) {
      case 'group':
        return !!(this.formData.trainerId && this.formData.trainingTypeId);
      case 'individual':
        return !!(this.formData.trainerId);
      case 'shift':
        return !!this.formData.employeeId;
      default:
        return false;
    }
  }

  private validateTimeRange(): void {
    if (this.formData.startTime && this.formData.endTime) {
      const start = new Date(this.formData.startTime);
      const end = new Date(this.formData.endTime);
      
      if (start.getHours() < 8) {
        start.setHours(8, 0, 0, 0);
        this.formData.startTime = start;
      }
      
      if (end.getHours() >= 22) {
        if (end.getHours() > 22 || (end.getHours() === 22 && end.getMinutes() > 0)) {
          end.setHours(22, 0, 0, 0);
          this.formData.endTime = end;
        }
      }
      
      if (end <= start) {
        end.setTime(start.getTime() + 60 * 60 * 1000);
        this.formData.endTime = end;
      }
    }
  }

  onTimeChange() {
    this.validateTimeRange();
  }

  getEventTypeLabel(): string {
    switch (this.selectedEventType) {
      case 'group': return 'Group Training';
      case 'individual': return 'Individual Training';
      case 'shift': return 'Staff Shift';
      default: return 'Event';
    }
  }

  onCancel() {
    this.cancelled.emit();
    this.closeModal();
  }

  onOverlayClick(event: MouseEvent) {
    this.onCancel();
  }

  private closeModal() {
    this.visible = false;
    this.visibleChange.emit(false);
    this.resetForm();
  }

  private resetForm() {
    this.currentStep = 1;
    this.selectedEventType = null;
    this.submitted = false;
    this.initializeFormData();
  }
}