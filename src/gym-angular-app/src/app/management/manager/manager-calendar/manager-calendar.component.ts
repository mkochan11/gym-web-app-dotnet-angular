// components/manager-calendar/manager-calendar.component.ts
import { Component } from '@angular/core';
import { SharedCalendarComponent } from '../../../shared/components/calendar/shared-calendar/shared-calendar.component';

@Component({
  selector: 'app-manager-calendar',
  standalone: true,
  imports: [SharedCalendarComponent],
  template: `
    <app-shared-calendar 
      [role]="'MANAGER'"
      (eventClick)="onEventClick($event)"
      (eventCreate)="onEventCreate($event)">
    </app-shared-calendar>
  `
})
export class ManagerCalendarComponent {
  onEventClick(event: any) {
    console.log('Manager event click:', event);
    // Manager-specific handling (edit all events)
  }

  onEventCreate(createInfo: any) {
    console.log('Manager create event:', createInfo);
    // Open event creation dialog with type selection
  }
}