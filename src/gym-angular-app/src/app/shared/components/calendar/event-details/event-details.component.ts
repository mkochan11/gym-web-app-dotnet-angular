import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { CardModule } from 'primeng/card';
import { ProgressBarModule } from 'primeng/progressbar';
import { CalendarEvent } from '../../../../core/models/calendar-event';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, ButtonModule, BadgeModule, CardModule, ProgressBarModule],
  templateUrl: './event-details.component.html',
  styleUrls: ['./event-details.component.scss']
})
export class EventDetailsComponent {
  @Input() event: CalendarEvent | null = null;
  @Input() eventType: 'group' | 'individual' | 'shift' | null = null;

  closeDetails() {
    this.event = null;
    this.eventType = null;
  }

  getDifficultyStars(difficulty: number): string {
    return '★'.repeat(difficulty) + '☆'.repeat(5 - difficulty);
  }

  getStatusBadgeSeverity() {
    if (!this.event) return 'secondary';
    
    if (this.event.isCancelled) return 'danger';
    if (this.event.isCompleted) return 'success';
    return 'info';
  }

  getStatusText() {
    if (!this.event) return '';
    
    if (this.event.isCancelled) return 'Cancelled';
    if (this.event.isCompleted) return 'Completed';
    return 'Scheduled';
  }
}