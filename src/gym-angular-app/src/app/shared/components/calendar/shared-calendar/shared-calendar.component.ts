import { Component, Input, OnInit, Output, EventEmitter, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, EventClickArg, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { Observable, Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { MultiSelectModule } from 'primeng/multiselect';
import { FormsModule } from '@angular/forms';

import { CalendarConfig, CALENDAR_CONFIGS } from '../../../../core/configurations/calendar-config';
import { AuthService, GroupTrainingService, IndividualTrainingService, ShiftService, EmployeeService, TrainingTypeService, ClientService } from '../../../../core/api-services';
import { CalendarEvent } from '../../../../core/models/shared-calendar/calendar-event';
import { EventDetailsComponent } from '../event-details/event-details.component';
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { Button } from "primeng/button";
import { Shift } from '../../../../core/models/shift';
import { IndividualTraining } from '../../../../core/models/individual-training';
import { GroupTraining } from '../../../../core/models/group-training';
import { CalendarFilters } from '../../../../core/models/shared-calendar/calendar-filters';
import { Employee } from '../../../../core/models/employee';
import { TrainingType } from '../../../../core/models/training-type';
import { Client } from '../../../../core/models/client';

@Component({
  selector: 'app-shared-calendar',
  standalone: true,
  imports: [
    CommonModule, 
    FullCalendarModule, 
    EventDetailsComponent, 
    ProgressSpinnerModule, 
    Button,
    MultiSelectModule,
    FormsModule
  ],
  templateUrl: './shared-calendar.component.html',
  styleUrls: ['./shared-calendar.component.scss']
})
export class SharedCalendarComponent implements OnInit, OnDestroy {
  private groupTrainingService = inject(GroupTrainingService);
  private individualTrainingService = inject(IndividualTrainingService);
  private shiftService = inject(ShiftService);
  private authService = inject(AuthService);
  private employeeService = inject(EmployeeService);
  private trainingTypeService = inject(TrainingTypeService);
  private clientService = inject(ClientService);
  private destroy$ = new Subject<void>();

  @Input() config: CalendarConfig = CALENDAR_CONFIGS['RECEPTIONIST'];
  @Input() showFilters = true;
  @Input() showDetailsPanelDefault = false;
  @Output() eventClick = new EventEmitter<CalendarEvent>();
  @Output() eventCreate = new EventEmitter<any>();
  @Output() eventEdit = new EventEmitter<CalendarEvent>();
  @Output() eventCancel = new EventEmitter<CalendarEvent>();
  @Output() eventDelete = new EventEmitter<CalendarEvent>();
  @Output() eventRestore = new EventEmitter<CalendarEvent>();

  calendarOptions!: CalendarOptions;
  
  selectedEvent: CalendarEvent | null = null;
  selectedEventType: 'group' | 'individual' | 'shift' | null = null;
  showDetailsPanel = false;
  isLoading = false;

  visibleEvents = {
    group: true,
    individual: true,
    shift: true
  }

  allEvents: EventInput[] = [];
  filteredEvents: EventInput[] = [];

  employeeOptions: any[] = [];
  trainingTypeOptions: any[] = [];
  clientOptions: any[] = [];
  
  selectedEmployees: number[] = [];
  selectedTrainingTypes: number[] = [];
  selectedClients: number[] = [];

  currentFilters: CalendarFilters = {
    eventTypes: ['group', 'individual', 'shift']
  };

  currentView: string = 'timeGridWeek';
  currentDateRange: { start: Date; end: Date } | null = null;

  filterSubject = new Subject<CalendarFilters>();

  ngOnInit() {
    if (!this.config) {
      const role = this.authService.getRole()?.toUpperCase() ?? 'RECEPTIONIST';
      this.config = CALENDAR_CONFIGS[role] || CALENDAR_CONFIGS['RECEPTIONIST'];
    }
    this.showDetailsPanel = this.showDetailsPanelDefault;
    this.currentFilters = this.getInitialFilters();
    this.visibleEvents = {
      group: this.config.canViewGroupTrainings,
      individual: this.config.canViewIndividualTrainings,
      shift: this.config.canViewShifts
    };
    this.initializeCalendar();
    this.setupFilterSubscription();
    this.loadFilterData();
    this.loadEvents();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ==================== FILTER METHODS ====================
  
  private setupFilterSubscription() {
    this.filterSubject.pipe(
      debounceTime(300),
      takeUntil(this.destroy$)
    ).subscribe(filters => {
      this.loadEvents(filters);
    });
  }

  updateFilters(newFilters: Partial<CalendarFilters>) {
    this.currentFilters = { ...this.currentFilters, ...newFilters };
    this.filterSubject.next(this.currentFilters);
  }

  clearFilters() {
    this.currentFilters = this.getInitialFilters();
    this.visibleEvents = {
      group: this.config.canViewGroupTrainings,
      individual: this.config.canViewIndividualTrainings,
      shift: this.config.canViewShifts
    };
    
    this.selectedEmployees = [];
    this.selectedTrainingTypes = [];
    this.selectedClients = [];
    
    this.filterSubject.next(this.currentFilters);
  }

  private getInitialFilters(): CalendarFilters {
    const eventTypes: ('group' | 'individual' | 'shift')[] = [];
    
    if (this.config.canViewGroupTrainings) eventTypes.push('group');
    if (this.config.canViewIndividualTrainings) eventTypes.push('individual');
    if (this.config.canViewShifts) eventTypes.push('shift');
    
    return { eventTypes };
  }

  // ==================== FILTER UI HANDLERS ====================

  onEmployeeFilterChange() {
    this.updateFilters({ 
      employeeIds: this.selectedEmployees.length > 0 ? this.selectedEmployees : undefined 
    });
  }

  onTrainingTypeFilterChange() {
    this.updateFilters({ 
      trainingTypeIds: this.selectedTrainingTypes.length > 0 ? this.selectedTrainingTypes : undefined 
    });
  }

  onClientFilterChange() {
    this.updateFilters({ 
      clientIds: this.selectedClients.length > 0 ? this.selectedClients : undefined 
    });
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  toggleEventVisibility(eventType: 'group' | 'individual' | 'shift') {
    this.visibleEvents[eventType] = !this.visibleEvents[eventType];
    this.applyEventFilter();
  }

  // ==================== CALENDAR INITIALIZATION ====================

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
      dayMaxEvents: 2,
      eventMaxStack: 2,
      moreLinkClick: 'popover',
      plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
      events: [],
      eventClick: this.handleEventClick.bind(this),
      select: this.handleDateSelect.bind(this),
      eventDrop: this.handleEventDrop.bind(this),
      eventResize: this.handleEventResize.bind(this),
      eventColor: this.getEventColor(this),
      eventDisplay: 'block',

      moreLinkText: (num) => `+${num} more`,

      selectAllow: (selectInfo) => {
        const now = new Date();
        const currentHour = new Date(now.getFullYear(), now.getMonth(), now.getDate(), now.getHours(), 0, 0, 0);
        return selectInfo.start >= currentHour;
      },

      eventAllow: (dropInfo, draggedEvent) => {
      const now = new Date();
      const eventStart = dropInfo.start;
      
      if (eventStart < now) {
        return false;
      }
      
      const originalEvent = draggedEvent?.extendedProps?.['originalData'];
      if (originalEvent && this.isEventCompletedOrOngoing(originalEvent)) {
        return false;
      }
      
      return true;
    },

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

  // ==================== EVENT HANDLERS (EMIT TO PARENT) ====================

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
      employee: extendedProps['employee'],
      capacity: extendedProps['capacity'],
      enrolled: extendedProps['enrolled'],
      description: extendedProps['description'],
      difficultyLevel: extendedProps['difficultyLevel'],
      trainingType: extendedProps['trainingType'],
      status: extendedProps['status'],
      originalData: extendedProps['originalData'],
      oldEnd: extendedProps['oldEnd'],
      oldStart: extendedProps['oldStart']
    }; 

    this.selectedEvent = calendarEvent;
    this.selectedEventType = extendedProps['type'];
    this.showDetailsPanel = true;
  }

  handleDateSelect(selectInfo: any) {
    if (!this.hasCreatePermissions()) return;

    this.eventCreate.emit({
      start: selectInfo.start,
      end: selectInfo.end,
    });
  }

  handleEventDrop(dropInfo: any) {
    const event = dropInfo.event;
    const extendedProps = event.extendedProps;
    const originalData = extendedProps['originalData'];
    const now = new Date();

    if (event.start! < now) {
      dropInfo.revert();
      return;
    }
    
    if (this.isEventCompletedOrOngoing(originalData)) {
      dropInfo.revert();
      return;
    }

    this.eventEdit.emit({
      ...this.createCalendarEventFromFullCalendarEvent(event),
      oldStart: new Date(originalData.date || originalData.startDate),
      oldEnd: new Date(originalData.endDate || new Date(new Date(originalData.date || originalData.startDate).getTime() + this.parseDuration(originalData.duration || '01:00:00')))
    });
  }

  handleEventResize(resizeInfo: any) {
    const event = resizeInfo.event;
    const extendedProps = event.extendedProps;
    const originalData = extendedProps['originalData'];
    const now = new Date();
    
      if (event.start! < now) {
      resizeInfo.revert();
      return;
    }

    if (this.isEventCompletedOrOngoing(originalData)) {
      resizeInfo.revert();
      return;
    }

    this.eventEdit.emit({
      ...this.createCalendarEventFromFullCalendarEvent(event),
      oldStart: new Date(originalData.date || originalData.startDate),
      oldEnd: new Date(originalData.endDate || new Date(new Date(originalData.date || originalData.startDate).getTime() + this.parseDuration(originalData.duration || '01:00:00')))
    });
  }

  // ==================== DETAILS PANEL HANDLERS ====================

  onEditEvent() {
    if (this.selectedEvent) {
      this.eventEdit.emit(this.selectedEvent);
      this.closeDetailsPanel();
    }
  }

  onCancelEvent() {
    if (this.selectedEvent) {
      this.eventCancel.emit(this.selectedEvent);
      this.closeDetailsPanel();
    }
  }

  onDeleteEvent() {
    if (this.selectedEvent) {
      this.eventDelete.emit(this.selectedEvent);
      this.closeDetailsPanel();
    }
  }

  onRestoreEvent() {
    if (this.selectedEvent) {
      this.eventRestore.emit(this.selectedEvent);
      this.closeDetailsPanel();
    }
  }

  closeDetailsPanel() {
    this.showDetailsPanel = false;
    this.selectedEvent = null;
    this.selectedEventType = null;
  }

  // ==================== DATA LOADING ====================

  private loadEvents(filters?: CalendarFilters) {
    this.isLoading = true;
    this.allEvents = [];

    const requests: Observable<any>[] = [];
    const eventTypesToLoad: ('group' | 'individual' | 'shift')[] = [];

    if (this.shouldLoadEventType('group', filters)) {
      eventTypesToLoad.push('group');
      requests.push(this.groupTrainingService.getGroupTrainingsFiltered(filters));
    }

    if (this.shouldLoadEventType('individual', filters)) {
      eventTypesToLoad.push('individual');
      requests.push(this.individualTrainingService.getIndividualTrainingsFiltered(filters));
    }

    if (this.shouldLoadEventType('shift', filters)) {
      eventTypesToLoad.push('shift');
      requests.push(this.shiftService.getShiftsFiltered(filters));
    }

    if (requests.length === 0) {
      this.allEvents = [];
      this.applyEventFilter();
      this.isLoading = false;
      return;
    }

    let completedRequests = 0;
    const totalRequests = requests.length;
    const loadedEvents: EventInput[] = [];

    requests.forEach((request, index) => {
      const eventType = eventTypesToLoad[index];
      
      request.subscribe({
        next: (data: any) => {
          let events: EventInput[] = [];
          
          switch (eventType) {
            case 'group':
              events = this.transformGroupTrainingsToEvents(data as GroupTraining[]);
              break;
            case 'individual':
              events = this.transformIndividualTrainingsToEvents(data as IndividualTraining[]);
              break;
            case 'shift':
              events = this.transformShiftsToEvents(data as Shift[]);
              console.log('Loaded shifts:', events);
              break;
          }
          
          loadedEvents.push(...events);
          completedRequests++;

          if (completedRequests === totalRequests) {
            this.allEvents = loadedEvents;
            this.applyEventFilter();
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.error(`Error loading ${eventType} events:`, error);
          completedRequests++;

          if (completedRequests === totalRequests) {
            this.allEvents = loadedEvents;
            this.applyEventFilter();
            this.isLoading = false;
          }
        }
      });
    });
  }

  private loadFilterData() {
    this.employeeService.getAllEmployees().subscribe({
      next: (employees: Employee[]) => {
        this.employeeOptions = employees.map(emp => ({
          id: emp.id,
          name: `${emp.firstName} ${emp.lastName}`
        }));
      },
      error: (error) => console.error('Error loading employees:', error)
    });

    if (this.config.canViewGroupTrainings) {
      this.trainingTypeService.getAllTrainingTypes().subscribe({
        next: (trainingTypes: TrainingType[]) => {
          this.trainingTypeOptions = trainingTypes.map(tt => ({
            id: tt.id,
            name: tt.name
          }));
        },
        error: (error) => console.error('Error loading training types:', error)
      });
    }

    if (this.config.canViewIndividualTrainings) {
      this.clientService.getAllClients().subscribe({
        next: (clients: Client[]) => {
          this.clientOptions = clients.map(client => ({
            id: client.id,
            name: `${client.firstName} ${client.lastName}`
          }));
        },
        error: (error) => console.error('Error loading clients:', error)
      });
    }
  }

  // ==================== HELPER METHODS ====================

  private shouldLoadEventType(eventType: 'group' | 'individual' | 'shift', filters?: CalendarFilters): boolean {
    switch (eventType) {
      case 'group':
        if (!this.config.canViewGroupTrainings) return false;
        break;
      case 'individual':
        if (!this.config.canViewIndividualTrainings) return false;
        break;
      case 'shift':
        if (!this.config.canViewShifts) return false;
        break;
    }

    if (!filters?.eventTypes) return true;
    return filters.eventTypes.includes(eventType);
  }

  private applyEventFilter() {
    this.filteredEvents = this.allEvents.filter(event => {
      const eventType = event.extendedProps?.['type'];
      return this.visibleEvents[eventType as keyof typeof this.visibleEvents];
    });
    
    this.calendarOptions.events = this.filteredEvents;
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

  private isEventCompletedOrOngoing(eventData: any): boolean {
    const status = eventData.status?.toLowerCase();
    const eventDate = new Date(eventData.date || eventData.startDate);
    const now = new Date();
    
    if (status === 'completed' || status === 'cancelled') {
      return true;
    }
    
    if (eventDate <= now) {
      return true;
    }
    
    return false;
  }

  private createCalendarEventFromFullCalendarEvent(event: any): CalendarEvent {
    const extendedProps = event.extendedProps;
    return {
      id: event.id,
      title: event.title,
      start: event.start!,
      end: event.end!,
      type: extendedProps['type'],
      trainer: extendedProps['trainer'],
      client: extendedProps['client'],
      employee: extendedProps['employee'],
      capacity: extendedProps['capacity'],
      enrolled: extendedProps['enrolled'],
      description: extendedProps['description'],
      difficultyLevel: extendedProps['difficultyLevel'],
      trainingType: extendedProps['trainingType'],
      status: extendedProps['status'],
      originalData: extendedProps['originalData'],
      oldEnd: extendedProps['oldEnd'],
      oldStart: extendedProps['oldStart']
    };
  }

  getEventColor(event: any): string {
    const eventType = event.type || event.extendedProps?.type;
    const status = event.status || event.extendedProps?.status;

    if (status === 'Cancelled') {
      return '#EF4444';
    } else if (status === 'Completed') {
      return '#9c9c9c';
    } 

    switch (eventType) {
      case 'individual': return '#3B82F6';
      case 'group': return '#10B981';
      case 'shift': return '#F59E0B';
      default: return '#6B7280';
    }
  }

  refreshCalendar() {
    this.loadEvents(this.currentFilters);
  }

  getPermissionsForEvent(): any {
  if (!this.selectedEventType) return {};

  const typeCapitalized = this.selectedEventType.charAt(0).toUpperCase() + this.selectedEventType.slice(1);
  const suffix = this.selectedEventType === 'shift' ? 's' : 'Trainings';

  const editKey = `canEdit${typeCapitalized}${suffix}` as keyof CalendarConfig;
  const deleteKey = `canDelete${typeCapitalized}${suffix}` as keyof CalendarConfig;

  return {
    canEdit: !!this.config[editKey],
    canCancel: !!this.config[editKey],
    canDelete: !!this.config[deleteKey],
    canRestore: !!this.config[editKey]
  };
}

  hasActiveFilters(): boolean {
    return !!(
      this.currentFilters.startDate ||
      this.currentFilters.endDate ||
      this.currentFilters.employeeIds?.length ||
      this.currentFilters.clientIds?.length ||
      this.currentFilters.trainingTypeIds?.length
    );
  }

  private transformGroupTrainingsToEvents(trainings: GroupTraining[]): EventInput[] {
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
        backgroundColor: this.getEventColor({ type: 'group', status: training.status }),
        borderColor: this.getEventColor({ type: 'group', status: training.status }),
        extendedProps: {
          type: 'group',
          trainer: trainerName,
          trainerId: training.trainer.id,
          trainingType: training.trainingType.name,
          trainingTypeId: training.trainingType.id,
          description: training.description,
          difficultyLevel: training.difficultyLevel,
          capacity: training.maxParticipantNumber,
          enrolled: training.currentParticipantNumber,
          status: training.status,
          originalData: training
        }
      };
    });
  }

  private transformIndividualTrainingsToEvents(trainings: IndividualTraining[]): EventInput[] {
    return trainings.map(training => {
      const startDate = new Date(training.date);
      const duration = this.parseDuration(training.duration);
      const endDate = new Date(startDate.getTime() + duration);
      
      const trainerName = `${training.trainer.firstName} ${training.trainer.lastName}`;
      const clientName = training.client 
      ? `${training.client.firstName} ${training.client.lastName}`
      : 'No client';
      
      return {
        id: `individual-${training.id}`,
        title: `PT - ${trainerName}`,
        start: startDate,
        end: endDate,
        backgroundColor: this.getEventColor({ type: 'individual', status : training.status }),
        borderColor: this.getEventColor({ type: 'individual', status : training.status }),
        extendedProps: {
          type: 'individual',
          trainer: trainerName,
          trainerId: training.trainer.id,
          client: clientName,
          clientId: training.client?.id || null,
          description: training.description,
          status: training.status,
          originalData: training
        }
      };
    });
  }

  private transformShiftsToEvents(shifts: Shift[]): EventInput[] {
    return shifts.map(shift => {
      const startDate = new Date(shift.startDate);
      const endDate = new Date(shift.endDate);
      
      const employeeName = `${shift.employee.firstName} ${shift.employee.lastName}`;
      
      return {
        id: `shift-${shift.id}`,
        title: `Shift - ${employeeName}`,
        start: startDate,
        end: endDate,
        backgroundColor: this.getEventColor({ type: 'shift', status: shift.status }),
        borderColor: this.getEventColor({ type: 'shift', status: shift.status }),
        extendedProps: {
          type: 'shift',
          employee: employeeName,
          employeeId: shift.employee.id,
          originalData: shift,
          status: shift.status
        }
      };
    });
  }

  private parseDuration(duration: string): number {
    const [hours, minutes, seconds] = duration.split(':').map(Number);
    return (hours * 60 * 60 * 1000) + (minutes * 60 * 1000) + (seconds * 1000);
  }
}