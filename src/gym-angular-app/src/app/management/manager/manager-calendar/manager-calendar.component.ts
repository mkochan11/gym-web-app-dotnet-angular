import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedCalendarComponent } from '../../../shared/components/calendar/shared-calendar/shared-calendar.component';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-manager-calendar',
  standalone: true,
  imports: [CommonModule, SharedCalendarComponent],
  template: `
    <app-shared-calendar 
      [role]="'MANAGER'"
      (eventClick)="onEventClick($event)"
      (eventCreate)="onEventCreate($event)"
      (eventEdit)="onEventEdit($event)"
      (eventCancel)="onEventCancel($event)"
      (eventDelete)="onEventDelete($event)">
    </app-shared-calendar>
  `
})
export class ManagerCalendarComponent {
  private toastService = inject(ToastService);

  onEventClick(event: any) {
    console.log('Manager event click:', event);
    // Manager-specific handling
  }

  onEventCreate(createInfo: any) {
    console.log('Manager create event:', createInfo);
    // Open event creation dialog
    this.toastService.show('Create event functionality would open here', 'info');
  }

  onEventEdit(event: any) {
    console.log('Manager edit event:', event);
    // Open event edit dialog
    this.toastService.show(`Edit event: ${event.title}`, 'info');
  }

  onEventCancel(event: any) {
    console.log('Manager cancel event:', event);
    // Implement cancel logic
    this.toastService.show(`Cancel event: ${event.title}`, 'info');
  }

  onEventDelete(event: any) {
    console.log('Manager delete event:', event);
    // Implement delete logic
    this.toastService.show(`Delete event: ${event.title}`, 'error');
  }
}