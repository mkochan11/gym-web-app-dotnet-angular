import { AfterViewInit, Component, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedCalendarComponent } from '../../../shared/components/calendar/shared-calendar/shared-calendar.component';
import { EventCancellationDialogComponent } from '../../../shared/components/calendar/event-cancellation-dialog/event-cancellation-dialog.component';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { GroupTrainingService, IndividualTrainingService, ShiftService } from '../../../core/api-services';

@Component({
  selector: 'app-manager-calendar',
  standalone: true,
  imports: [
    CommonModule, 
    SharedCalendarComponent, 
    EventCancellationDialogComponent,
    ConfirmDialogModule
  ],
  providers: [ConfirmationService, MessageService],
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

  ngAfterViewInit(): void {}

  onEventCreate(createInfo: any) {
    console.log('Manager: Create event', createInfo);
    this.toastService.show('Create event functionality would open here', 'info');
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