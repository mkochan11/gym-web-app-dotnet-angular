import { Component, Input, OnInit, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, EventClickArg, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';

import { CalendarConfig, CALENDAR_CONFIGS } from '../../../../core/configurations/calendar-config';
import { GroupTrainingService } from '../../../../core/api-services';
import { CalendarEvent } from '../../../../core/models/calendar-event';
import { EventDetailsComponent } from '../event-details/event-details.component';
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { Button } from "primeng/button";

@Component({
  selector: 'app-shared-calendar',
  standalone: true,
  imports: [CommonModule, FullCalendarModule, EventDetailsComponent, ProgressSpinnerModule, Button],
  templateUrl: './shared-calendar.component.html',
  styleUrls: ['./shared-calendar.component.scss']
})
export class SharedCalendarComponent implements OnInit {
  private groupTrainingService = inject(GroupTrainingService);

  @Input() role: string = 'RECEPTIONIST';
  @Output() eventClick = new EventEmitter<CalendarEvent>();
  @Output() eventCreate = new EventEmitter<any>();
  @Output() eventEdit = new EventEmitter<CalendarEvent>();
  @Output() eventCancel = new EventEmitter<CalendarEvent>();
  @Output() eventDelete = new EventEmitter<CalendarEvent>();

  config!: CalendarConfig;
  calendarOptions!: CalendarOptions;
  
  selectedEvent: CalendarEvent | null = null;
  selectedEventType: 'group' | 'individual' | 'shift' | null = null;
  showDetailsPanel = false;
  isLoading = false;

  ngOnInit() {
    this.config = CALENDAR_CONFIGS[this.role] || CALENDAR_CONFIGS['RECEPTIONIST'];
    this.initializeCalendar();
    this.loadEvents();
  }

  private initializeCalendar() {
    this.calendarOptions = {
      initialView: this.config.defaultView,
      headerToolbar: {
        left: 'prev,next today',
        center: 'title',
        right: this.config.allowedViews.join(',')
      },
      weekends: true,
      editable: this.hasEditPermissions(),
      selectable: this.hasCreatePermissions(),
      selectMirror: true,
      dayMaxEvents: true,
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      events: [],
      eventClick: this.handleEventClick.bind(this),
      select: this.handleDateSelect.bind(this),
      eventColor: this.getEventColor(this),
      eventDisplay: 'block',
      
      slotMinTime: '07:00:00',
      slotMaxTime: '22:00:00',
      allDaySlot: false,
      slotLabelFormat: {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
      },
      
      height: 'auto',
      contentHeight: 'auto',
      locale: 'en',
      firstDay: 1,
    };
  }

  private hasEditPermissions(): boolean {
    return this.config.canEditIndividualTrainings || 
           this.config.canEditGroupTrainings || 
           this.config.canEditShifts;
  }

  private hasCreatePermissions(): boolean {
    return this.config.canCreateIndividualTrainings || 
           this.config.canCreateGroupTrainings || 
           this.config.canCreateShifts;
  }

  private loadEvents() {
    this.isLoading = true;
    
    this.groupTrainingService.getAllGroupTrainings().subscribe({
      next: (trainings) => {
        const events = this.transformGroupTrainingsToEvents(trainings);
        this.calendarOptions.events = events;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading events:', error);
        this.isLoading = false;
      }
    });
  }

  private transformGroupTrainingsToEvents(trainings: any[]): EventInput[] {
    return trainings.map(training => {
      const startDate = new Date(training.date);
      const duration = this.parseDuration(training.duration);
      const endDate = new Date(startDate.getTime() + duration);
      
      const trainerName = `${training.trainer.firstName} ${training.trainer.lastName}`;
      
      return {
        id: `group-${training.id}`,
        title: `${training.trainingType.name} - ${trainerName}`,
        start: startDate,
        end: endDate,
        backgroundColor: this.getEventColor({ type: 'group' }),
        borderColor: this.getEventColor({ type: 'group' }),
        extendedProps: {
          type: 'group',
          trainer: trainerName,
          trainingType: training.trainingType.name,
          description: training.description,
          difficultyLevel: training.difficultyLevel,
          capacity: training.maxParticipantNumber,
          enrolled: training.currentParticipantNumber,
          isCompleted: training.isCompleted,
          isCancelled: training.isCancelled,
          originalData: training
        }
      };
    });
  }

  private parseDuration(duration: string): number {
    const [hours, minutes, seconds] = duration.split(':').map(Number);
    return (hours * 60 * 60 * 1000) + (minutes * 60 * 1000) + (seconds * 1000);
  }

  getEventColor(event: any): string {
    const eventType = event.type || event.extendedProps?.type;
    
    switch (eventType) {
      case 'individual': return '#3B82F6';
      case 'group': return '#10B981';
      case 'shift': return '#F59E0B';
      default: return '#6B7280';
    }
  }

  handleEventClick(clickInfo: EventClickArg) {
    const event = clickInfo.event;
    const extendedProps = event.extendedProps;
    
    const calendarEvent: CalendarEvent = {
      id: event.id,
      title: event.title,
      start: event.start!,
      end: event.end!,
      type: extendedProps['type'],
      trainer: extendedProps['trainer'],
      client: extendedProps['client'],
      capacity: extendedProps['capacity'],
      enrolled: extendedProps['enrolled'],
      description: extendedProps['description'],
      difficultyLevel: extendedProps['difficultyLevel'],
      trainingType: extendedProps['trainingType'],
      isCompleted: extendedProps['isCompleted'],
      isCancelled: extendedProps['isCancelled'],
      originalData: extendedProps['originalData']
    };

    this.selectedEvent = calendarEvent;
    this.selectedEventType = extendedProps['type'];
    this.showDetailsPanel = true;
    this.eventClick.emit(calendarEvent);
  }

  handleDateSelect(selectInfo: any) {
    if (!this.hasCreatePermissions()) return;

    this.eventCreate.emit({
      start: selectInfo.start,
      end: selectInfo.end,
      allDay: selectInfo.allDay
    });
  }

  closeDetailsPanel() {
    this.showDetailsPanel = false;
    this.selectedEvent = null;
    this.selectedEventType = null;
  }

  onEditEvent() {
    if (this.selectedEvent) {
      this.eventEdit.emit(this.selectedEvent);
    }
  }

  onCancelEvent() {
    if (this.selectedEvent) {
      this.eventCancel.emit(this.selectedEvent);
    }
  }

  onDeleteEvent() {
    if (this.selectedEvent) {
      this.eventDelete.emit(this.selectedEvent);
    }
  }

  refreshCalendar() {
    this.loadEvents();
  }

  getPermissionsForEvent(): any {
    if (!this.selectedEventType) return {};
    
    return {
      canEdit: this.config[`canEdit${this.selectedEventType.charAt(0).toUpperCase() + this.selectedEventType.slice(1)}Trainings` as keyof CalendarConfig] as boolean,
      canCancel: this.config[`canEdit${this.selectedEventType.charAt(0).toUpperCase() + this.selectedEventType.slice(1)}Trainings` as keyof CalendarConfig] as boolean,
      canDelete: this.config[`canDelete${this.selectedEventType.charAt(0).toUpperCase() + this.selectedEventType.slice(1)}Trainings` as keyof CalendarConfig] as boolean,
    };
  }
}