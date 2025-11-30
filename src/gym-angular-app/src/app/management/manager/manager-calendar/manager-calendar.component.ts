import { AfterViewInit, Component, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedCalendarComponent } from '../../../shared/components/calendar/shared-calendar/shared-calendar.component';
import { EventCancellationDialogComponent } from '../../../shared/components/calendar/event-cancellation-dialog/event-cancellation-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { GroupTrainingService, IndividualTrainingService, ShiftService } from '../../../core/api-services';
import { DialogModule } from 'primeng/dialog';
import { DialogService } from 'primeng/dynamicdialog';
import { ManagerCalendarEventAddModalComponent } from './manager-calendar-event-add/manager-calendar-event-add.component';

@Component({
  selector: 'app-manager-calendar',
  standalone: true,
  imports: [
    CommonModule, 
    SharedCalendarComponent, 
    EventCancellationDialogComponent,
    ConfirmDialogModule,
    DialogModule,
    ManagerCalendarEventAddModalComponent
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

    <app-manager-calendar-event-add-modal
      [(visible)]="showEventModal"
      [startTime]="creationStartTime"
      [endTime]="creationEndTime"
      (eventCreated)="onEventCreated($event)"
      (cancelled)="onEventCreationCancelled()">
    </app-manager-calendar-event-add-modal>

    <app-shared-calendar 
      [role]="'MANAGER'"
      (eventCreate)="onEventCreate($event)"
      (eventEdit)="onEventEdit($event)"
      (eventCancel)="onEventCancel($event)"
      (eventDelete)="onEventDelete($event)">
    </app-shared-calendar>
  `
})

export class ManagerCalendarComponent implements AfterViewInit {
  @ViewChild(SharedCalendarComponent) sharedCalendar!: SharedCalendarComponent;

  private toastService = inject(ToastService);
  private confirmationService = inject(ConfirmationService);
  private groupTrainingService = inject(GroupTrainingService);
  private individualTrainingService = inject(IndividualTrainingService);
  private shiftService = inject(ShiftService);

  showCancelDialog = false;
  selectedEvent: any = null;
  isCancelling = false;

  showEventModal = false;
  creationStartTime!: Date;
  creationEndTime!: Date;
  availableEventTypes: any[] = [];

  ngAfterViewInit(): void {}

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
    console.log('Event created:', eventData);
    this.createEvent(eventData);
    this.showEventModal = false;
  }

  onEventCreationCancelled() {
    console.log('Event creation cancelled by user');
    this.showEventModal = false;
  }

  onEventEdit(event: any) {
    console.log('Manager: Edit event', event);
    this.toastService.show(`Edit event: ${event.title}`, 'info');
  }

  onEventCancel(event: any) {
    console.log('Manager: Cancel event received', event);
    
    if (!event) {
      console.error('No event provided for cancellation');
      this.toastService.show('No event selected for cancellation', 'error');
      return;
    }

    this.selectedEvent = event;
    this.showCancelDialog = true;
  }

  onEventDelete(event: any) {
    console.log('Manager: Delete event', event);
    
    this.confirmationService.confirm({
      message: `Are you sure you want to delete "${event.title}"? This action cannot be undone.`,
      header: 'Confirm Deletion',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.performEventDeletion(event);
      }
    });
  }

  onCancellationConfirmed(cancellationReason: string) {
    this.isCancelling = true;
    this.performEventCancellation(this.selectedEvent, cancellationReason);
  }

  onCancellationCancelled() {
    console.log('Cancellation was cancelled by user');
    this.resetCancellationState();
  }

  private createEvent(eventData: any) {
    let creationObservable;

    console.log(`Create Event with`, eventData)
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
        this.toastService.show(`Failed to create event. "${error}"`, 'error');
        this.isCancelling = false;
      }
    });
  }

  private resetCancellationState() {
    this.selectedEvent = null;
    this.isCancelling = false;
  }

  private performEventCancellation(event: any, reason: string) {
    let cancellationObservable;

    switch (event.type) {
      case 'group':
        cancellationObservable = this.groupTrainingService.cancelGroupTraining(
          this.extractId(event.id), 
          reason
        );
        break;
      case 'individual':
        cancellationObservable = this.individualTrainingService.cancelIndividualTraining(
          this.extractId(event.id), 
          reason
        );
        break;
      case 'shift':
        cancellationObservable = this.shiftService.cancelShift(
          this.extractId(event.id), 
          reason
        );
        break;
      default:
        this.toastService.show('Unknown event type', 'error');
        this.isCancelling = false;
        return;
    }

    cancellationObservable.subscribe({
      next: (response) => {
        this.toastService.show(`Event "${event.title}" has been cancelled successfully`, 'success');
        this.showCancelDialog = false;
        this.resetCancellationState();

        this.sharedCalendar.refreshCalendar();
      },
      error: (error) => {
        this.toastService.show(`Failed to cancel event. "${error}"`, 'error');
        this.isCancelling = false;
      }
    });
  }

  private performEventDeletion(event: any) {
    setTimeout(() => {
      this.toastService.show(`Event "${event.title}" has been deleted successfully`, 'success');
    }, 1000);
  }

  private extractId(eventId: string): number {
    return parseInt(eventId.split('-')[1]);
  }
}