export interface CalendarConfig {
  canViewIndividualTrainings: boolean;
  canViewGroupTrainings: boolean;
  canViewShifts: boolean;

  canEditIndividualTrainings: boolean;
  canEditGroupTrainings: boolean;
  canEditShifts: boolean;

  canCreateIndividualTrainings: boolean;
  canCreateGroupTrainings: boolean;
  canCreateShifts: boolean;

  canDeleteIndividualTrainings: boolean;
  canDeleteGroupTrainings: boolean;
  canDeleteShifts: boolean;

  canReserveTrainings?: boolean;
  canCancelReservations?: boolean;

  showCreateButton: boolean;
  allowedViews: string[];
  defaultView: string;
}

export const CALENDAR_CONFIGS: { [role: string]: CalendarConfig } = {
  RECEPTIONIST: {
    canViewIndividualTrainings: true,
    canViewGroupTrainings: true,
    canViewShifts: true,
    canEditIndividualTrainings: false,
    canEditGroupTrainings: false,
    canEditShifts: false,
    canCreateIndividualTrainings: false,
    canCreateGroupTrainings: false,
    canCreateShifts: false,
    canDeleteIndividualTrainings: false,
    canDeleteGroupTrainings: false,
    canDeleteShifts: false,
    showCreateButton: false,
    allowedViews: ['timeGridWeek', 'timeGridDay'],
    defaultView: 'timeGridWeek'
  },
  TRAINER: {
    canViewIndividualTrainings: true,
    canViewGroupTrainings: true,
    canViewShifts: false,
    canEditIndividualTrainings: true,
    canEditGroupTrainings: true,
    canEditShifts: false,
    canCreateIndividualTrainings: true,
    canCreateGroupTrainings: true,
    canCreateShifts: false,
    canDeleteIndividualTrainings: true,
    canDeleteGroupTrainings: true,
    canDeleteShifts: false,
    showCreateButton: true,
    allowedViews: ['timeGridWeek', 'timeGridDay', 'dayGridMonth'],
    defaultView: 'timeGridWeek'
  },
  MANAGER: {
    canViewIndividualTrainings: true,
    canViewGroupTrainings: true,
    canViewShifts: true,
    canEditIndividualTrainings: true,
    canEditGroupTrainings: true,
    canEditShifts: true,
    canCreateIndividualTrainings: true,
    canCreateGroupTrainings: true,
    canCreateShifts: true,
    canDeleteIndividualTrainings: true,
    canDeleteGroupTrainings: true,
    canDeleteShifts: true,
    showCreateButton: true,
    allowedViews: ['timeGridWeek', 'timeGridDay', 'dayGridMonth'],
    defaultView: 'timeGridWeek'
  },
  OWNER: {
    canViewIndividualTrainings: true,
    canViewGroupTrainings: true,
    canViewShifts: true,
    canEditIndividualTrainings: true,
    canEditGroupTrainings: true,
    canEditShifts: true,
    canCreateIndividualTrainings: true,
    canCreateGroupTrainings: true,
    canCreateShifts: true,
    canDeleteIndividualTrainings: true,
    canDeleteGroupTrainings: true,
    canDeleteShifts: true,
    showCreateButton: true,
    allowedViews: ['timeGridWeek', 'timeGridDay', 'dayGridMonth'],
    defaultView: 'timeGridWeek'
  },
  CLIENT: {
    canViewIndividualTrainings: true,
    canViewGroupTrainings: true,
    canViewShifts: false,
    canEditIndividualTrainings: false,
    canEditGroupTrainings: false,
    canEditShifts: false,
    canCreateIndividualTrainings: false,
    canCreateGroupTrainings: false,
    canCreateShifts: false,
    canDeleteIndividualTrainings: false,
    canDeleteGroupTrainings: false,
    canDeleteShifts: false,
    showCreateButton: false,
    allowedViews: ['timeGridWeek', 'timeGridDay', 'dayGridMonth'],
    defaultView: 'timeGridWeek',
    
    canReserveTrainings: true,
    canCancelReservations: true
  }
};