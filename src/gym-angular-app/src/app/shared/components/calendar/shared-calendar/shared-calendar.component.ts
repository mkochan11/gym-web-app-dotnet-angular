import { Component, Input, OnInit, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, EventClickArg, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';

import { CalendarConfig, CALENDAR_CONFIGS } from '../../../../core/configurations/calendar-config';
import { GroupTrainingService, IndividualTrainingService, ShiftService } from '../../../../core/api-services';
import { CalendarEvent } from '../../../../core/models/calendar-event';
import { EventDetailsComponent } from '../event-details/event-details.component';

@Component({
  selector: 'app-shared-calendar',
  standalone: true,
  imports: [CommonModule, FullCalendarModule, EventDetailsComponent],
  templateUrl: './shared-calendar.component.html',
  styleUrls: ['./shared-calendar.component.scss']
})
export class SharedCalendarComponent implements OnInit {
  private groupTrainingService = inject(GroupTrainingService);
  private individualTrainingService = inject(IndividualTrainingService);
  private shiftService = inject(ShiftService);

  @Input() role: string = 'RECEPTIONIST';
  @Output() eventClick = new EventEmitter<CalendarEvent>();
  @Output() eventCreate = new EventEmitter<any>();

  config!: CalendarConfig;
  calendarOptions!: CalendarOptions;
  
  selectedEvent: CalendarEvent | null = null;
  showDetailsPanel = false;
  isLoading = false;

  ngOnInit() {
    this.config = CALENDAR_CONFIGS[this.role] || CALENDAR_CONFIGS.RECEPTIONIST;
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
      
      slotMinTime: '06:00:00',
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

  private async loadEvents() {
    this.isLoading = true;
    
    try {
      const events: EventInput[] = [];
      
      if (this.config.canViewGroupTrainings) {
        const groupTrainings = await this.groupTrainingService.getAllGroupTrainings().toPromise();
        events.push(...this.transformGroupTrainingsToEvents(groupTrainings || []));
      }

      // if (this.config.canViewIndividualTrainings) {
      //   const individualTrainings = await this.individualTrainingService.getUpcomingIndividualTrainings().toPromise();
      //   events.push(...this.transformIndividualTrainingsToEvents(individualTrainings || []));
      // }

      // // Load shifts if permitted
      // if (this.config.canViewShifts) {
      //   const shifts = await this.shiftService.getUpcomingShifts().toPromise();
      //   events.push(...this.transformShiftsToEvents(shifts || []));
      // }

      this.calendarOptions.events = events;
    } catch (error) {
      console.error('Error loading events:', error);
    } finally {
      this.isLoading = false;
    }
  }

  private transformGroupTrainingsToEvents(trainings: any[]): EventInput[] {
    return trainings.map(training => ({
      id: `group-${training.id}`,
      title: `${training.trainingType?.name || 'Group Training'} - ${training.trainer?.firstName} ${training.trainer?.lastName}`,
      start: new Date(training.date),
      end: new Date(new Date(training.date).getTime() + this.parseDuration(training.duration)),
      backgroundColor: '#10B981',
      borderColor: '#10B981',
      extendedProps: {
        type: 'group',
        trainer: `${training.trainer?.firstName} ${training.trainer?.lastName}`,
        trainingType: training.trainingType?.name,
        description: training.description,
        difficultyLevel: training.difficultyLevel,
        capacity: training.maxParticipantNumber,
        enrolled: training.currentParticipantNumber,
        isCompleted: training.isCompleted,
        isCancelled: training.isCancelled,
        originalData: training
      }
    }));
  }

  private transformIndividualTrainingsToEvents(trainings: any[]): EventInput[] {
    return trainings.map(training => ({
      id: `individual-${training.id}`,
      title: `PT - ${training.client?.firstName} ${training.client?.lastName}`,
      start: new Date(training.date),
      end: new Date(new Date(training.date).getTime() + this.parseDuration(training.duration)),
      backgroundColor: '#3B82F6',
      borderColor: '#3B82F6',
      extendedProps: {
        type: 'individual',
        trainer: `${training.trainer?.firstName} ${training.trainer?.lastName}`,
        client: `${training.client?.firstName} ${training.client?.lastName}`,
        location: training.location,
        description: training.notes,
        isCompleted: training.isCompleted,
        isCancelled: training.isCancelled,
        originalData: training
      }
    }));
  }

  private transformShiftsToEvents(shifts: any[]): EventInput[] {
    return shifts.map(shift => ({
      id: `shift-${shift.id}`,
      title: `Shift - ${shift.employee?.firstName} ${shift.employee?.lastName}`,
      start: new Date(shift.startTime),
      end: new Date(shift.endTime),
      backgroundColor: '#F59E0B',
      borderColor: '#F59E0B',
      extendedProps: {
        type: 'shift',
        trainer: `${shift.employee?.firstName} ${shift.employee?.lastName}`,
        location: shift.position,
        description: shift.notes,
        originalData: shift
      }
    }));
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
      location: extendedProps['location'],
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
  }

  refreshCalendar() {
    this.loadEvents();
  }
}