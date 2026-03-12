import { AfterViewInit, Component, inject, ViewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedCalendarComponent } from '../../../shared/components/calendar/shared-calendar/shared-calendar.component';
import { EventCancellationDialogComponent } from '../../../shared/components/calendar/event-cancellation-dialog/event-cancellation-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { GroupTrainingService, IndividualTrainingService, ShiftService } from '../../../core/api-services';
import { DialogModule } from 'primeng/dialog';
import { DialogService } from 'primeng/dynamicdialog';
import { CalendarService } from '../../../core/services/calendar-service';
import { EventAddModalComponent, EventTypeOption, EmployeeOption, TrainingTypeOption } from '../../../shared/components/calendar/event-add-modal/event-add-modal.component';
import { EventEditModalComponent } from '../../../shared/components/calendar/event-edit-modal/event-edit-modal.component';
import { EmployeeService, TrainingTypeService } from '../../../core/api-services';
import { Employee } from '../../../core/models/employee';
import { TrainingType } from '../../../core/models/training-type';
import { CALENDAR_CONFIGS, CalendarConfig } from '../../../core/configurations/calendar-config';

@Component({
  selector: 'app-manager-calendar',
  standalone: true,
  imports: [
    CommonModule, 
    SharedCalendarComponent, 
    EventCancellationDialogComponent,
    ConfirmDialogModule,
    DialogModule,
    EventAddModalComponent,
    EventEditModalComponent
  ],
  providers: [ConfirmationService, MessageService, DialogService],
  template: `
    <p-confirmDialog 
      [style]="{width: '450px'}"
      [baseZIndex]="10000">
    </p-confirmDialog>

    <app-event-cancellation-dialog
      [(visible)]="showCancelDialog"
      [eventTitle]="selectedEvent?.title || ''"
      [loading]="isCancelling"
      (confirmed)="onCancellationConfirmed($event)"
      (cancelled)="onCancellationCancelled()">
    </app-event-cancellation-dialog>

    <app-event-add-modal
      [(visible)]="showEventModal"
      [startTime]="creationStartTime"
      [endTime]="creationEndTime"
      [availableEventTypes]="availableEventTypes"
      [employeeOptions]="employeeOptions"
      [trainingTypeOptions]="trainingTypeOptions"
      (eventCreated)="onEventCreated($event)"
      (cancelled)="onEventCreationCancelled()">
    </app-event-add-modal>

    <app-event-edit-modal
      [(visible)]="showEditModal"
      [event]="selectedEventForEdit"
      [employeeOptions]="employeeOptions"
      [trainingTypeOptions]="trainingTypeOptions"
      (eventUpdated)="onEventUpdated($event)"
      (cancelled)="onEventEditCancelled()">
    </app-event-edit-modal>

    <app-shared-calendar 
      [config]="managerConfig"
      (eventCreate)="onEventCreate($event)"
      (eventEdit)="onEventEdit($event)"
      (eventCancel)="onEventCancel($event)"
      (eventDelete)="onEventDelete($event)"
      (eventRestore)="onEventRestore($event)">
    </app-shared-calendar>
  `
})
export class ManagerCalendarComponent implements AfterViewInit, OnInit {
  @ViewChild(SharedCalendarComponent) sharedCalendar!: SharedCalendarComponent;

  private toastService = inject(ToastService);
  private confirmationService = inject(ConfirmationService);
  private groupTrainingService = inject(GroupTrainingService);
  private individualTrainingService = inject(IndividualTrainingService);
  private shiftService = inject(ShiftService);
  private calendarService = inject(CalendarService);
  private employeeService = inject(EmployeeService);
  private trainingTypeService = inject(TrainingTypeService);

  managerConfig: CalendarConfig = CALENDAR_CONFIGS['MANAGER'];

  showCancelDialog = false;
  selectedEvent: any = null;
  isCancelling = false;

  showEventModal = false;
  showEditModal = false;
  creationStartTime!: Date;
  creationEndTime!: Date;
  selectedEventForEdit: any = null;

  availableEventTypes: EventTypeOption[] = [
    { value: 'group', label: 'Group Training', description: 'Training session for multiple participants', color: '#10B981', icon: 'pi pi-users' },
    { value: 'individual', label: 'Individual Training', description: 'One-on-one personal training session', color: '#3B82F6', icon: 'pi pi-user' },
    { value: 'shift', label: 'Staff Shift', description: 'Employee work schedule', color: '#F59E0B', icon: 'pi pi-briefcase' }
  ];

  employeeOptions: EmployeeOption[] = [];
  trainingTypeOptions: TrainingTypeOption[] = [];

  ngOnInit() {
    this.loadDropdownData();
  }

  ngAfterViewInit(): void {}

  private loadDropdownData() {
    this.employeeService.getAllEmployees().subscribe({
      next: (employees: Employee[]) => {
        this.employeeOptions = employees.map(emp => ({
          id: emp.id,
          name: `${emp.firstName} ${emp.lastName}`,
          role: emp.role
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

  async onEventCreate(createInfo: any) {
    try {
      this.creationStartTime = new Date(createInfo.start);
      this.creationEndTime = new Date(createInfo.end);
      setTimeout(() => {
        this.showEventModal = true;
      }, 0);
    } catch (error: any) {
      this.toastService.show('Failed to start event creation. Please try again.', 'error');
    }
  }

  onEventCreated(eventData: any) {
    this.createEvent(eventData);
    this.showEventModal = false;
  }

  onEventCreationCancelled() {
    this.showEventModal = false;
  }

  onEventEdit(event: any) {
    if (!event) {
      this.toastService.show('No event selected for editing', 'error');
      return;
    }
    this.selectedEventForEdit = event;
    setTimeout(() => {
      this.showEditModal = true;
    }, 0);
  }

  onEventUpdated(eventData: any) {
    this.updateEvent(eventData);
    this.showEditModal = false;
  }

  onEventEditCancelled() {
    this.showEditModal = false;
    this.selectedEventForEdit = null;
  }

  onEventCancel(event: any) {
    if (!event) {
      this.toastService.show('No event selected for cancellation', 'error');
      return;
    }
    this.selectedEvent = event;
    this.showCancelDialog = true;
  }

  onEventDelete(event: any) {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete "${event.title}"? This action cannot be undone.`,
      header: 'Confirm Deletion',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.performEventDeletion(event);
      }
    });
  }

  onEventRestore(event: any) {
    this.confirmationService.confirm({
      message: `Are you sure you want to restore "${event.title}"?`,
      header: 'Confirm Restoration',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.performEventRestore(event);
      }
    });
  }

  onCancellationConfirmed(cancellationReason: string) {
    this.isCancelling = true;
    this.performEventCancellation(this.selectedEvent, cancellationReason);
  }

  onCancellationCancelled() {
    this.resetCancellationState();
  }

  private createEvent(eventData: any) {
    let creationObservable;

    switch (eventData.type) {
      case 'group':
        creationObservable = this.groupTrainingService.createGroupTraining(eventData);
        break;
      case 'individual':
        creationObservable = this.individualTrainingService.createIndividualTraining(eventData);
        break;
      case 'shift':
        creationObservable = this.shiftService.createShift(eventData);
        break;
      default:
        this.toastService.show('Unknown event type', 'error');
        this.showEventModal = false;
        return;
    }

    creationObservable.subscribe({
      next: (response) => {
        this.toastService.show(`Event has been created successfully`, 'success');
        this.showCancelDialog = false;
        this.resetCancellationState();
        this.sharedCalendar.refreshCalendar();
      },
      error: (error) => {
        this.toastService.show(`Failed to create event: ${error.message}`, 'error');
        this.isCancelling = false;
      }
    });
  }

  private updateEvent(eventData: any) {
    let updateObservable;

    switch (eventData.type) {
      case 'group':
        updateObservable = this.groupTrainingService.updateGroupTraining(eventData.id, eventData);
        break;
      case 'individual':
        updateObservable = this.individualTrainingService.updateIndividualTraining(eventData.id, eventData);
        break;
      case 'shift':
        updateObservable = this.shiftService.updateShift(eventData.id, eventData);
        break;
      default:
        this.toastService.show('Unknown event type', 'error');
        return;
    }

    updateObservable.subscribe({
      next: (response) => {
        this.toastService.show(`Event has been updated successfully`, 'success');
        this.selectedEventForEdit = null;
        this.sharedCalendar.refreshCalendar();
      },
      error: (error) => {
        this.toastService.show(`Failed to update event: ${error.message}`, 'error');
      }
    });
  }

  private resetCancellationState() {
    this.selectedEvent = null;
    this.isCancelling = false;
  }

  private performEventCancellation(event: any, reason: string) {
    this.calendarService.cancelEvent(event.type, event.id, reason).subscribe({
      next: () => {
        this.toastService.show(`Event "${event.title}" cancelled`, 'success');
        this.showCancelDialog = false;
        this.resetCancellationState();
        this.sharedCalendar.refreshCalendar();
      },
      error: (err) => this.toastService.show(err.message, 'error')
    });
  }

  private performEventDeletion(event: any) {
    this.calendarService.deleteEvent(event.type, event.id).subscribe({
      next: () => {
        this.toastService.show(`Event "${event.title}" deleted`, 'success');
        this.sharedCalendar.refreshCalendar();
      },
      error: (err) => this.toastService.show(err.message, 'error')
    });
  }

  private performEventRestore(event: any) {
    this.calendarService.restoreEvent(event.type, event.id).subscribe({
      next: () => {
        this.toastService.show(`Event "${event.title}" has been restored successfully`, 'success');
        this.sharedCalendar.refreshCalendar();
        this.resetCancellationState();
      },
      error: (error) => {
        this.toastService.show(`Failed to restore event: ${error.message}`, 'error');
      }
    });
  }
}
