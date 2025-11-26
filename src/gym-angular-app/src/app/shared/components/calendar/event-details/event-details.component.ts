import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { BadgeModule } from 'primeng/badge';
import { CardModule } from 'primeng/card';
import { ProgressBarModule } from 'primeng/progressbar';
import { CalendarEvent } from '../../../../core/models/shared-calendar/calendar-event';

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
  @Input() permissions: any = {};
  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  @Output() delete = new EventEmitter<void>();

  closeDetails() {
    this.close.emit();
  }

  editEvent() {
    this.edit.emit();
  }

  cancelEvent() {
    this.cancel.emit();
  }

  deleteEvent() {
    this.delete.emit();
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

  getDuration(): string {
    if (!this.event) return '';
    const duration = this.event.end.getTime() - this.event.start.getTime();
    const hours = Math.floor(duration / (1000 * 60 * 60));
    const minutes = Math.floor((duration % (1000 * 60 * 60)) / (1000 * 60));
    
    if (hours > 0) {
      return `${hours}h ${minutes > 0 ? `${minutes}m` : ''}`;
    }
    return `${minutes}m`;
  }

  getAvailabilityPercentage(): number {
    if (!this.event || !this.event.capacity || !this.event.enrolled) return 0;
    return (this.event.enrolled / this.event.capacity) * 100;
  }

  isTrainingFull(): boolean {
    if (!this.event || !this.event.capacity || !this.event.enrolled) return false;
    return this.event.enrolled >= this.event.capacity;
  }
}