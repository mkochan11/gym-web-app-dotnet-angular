export interface CalendarEvent {
    id: string;
    title: string;
    start: Date;
    end: Date;
    type: 'individual' | 'group' | 'shift';
    trainer?: string;
    client?: string;
    employee?: string;
    capacity?: number;
    enrolled?: number;
    description?: string;
    difficultyLevel?: number;
    trainingType?: string;
    isCompleted?: boolean;
    isCancelled?: boolean;
    originalData: any;
}
