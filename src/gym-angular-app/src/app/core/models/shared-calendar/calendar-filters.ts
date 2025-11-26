export interface CalendarFilters {
    startDate?: Date;
    endDate?: Date;
    employeeIds?: number[];
    clientIds?: number[];
    trainingTypeIds?: number[];
    eventTypes?: ('group' | 'individual' | 'shift')[];
}
