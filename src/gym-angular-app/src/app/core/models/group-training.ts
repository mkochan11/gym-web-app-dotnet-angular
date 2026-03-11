import { Trainer } from "./trainer";
import { TrainingType } from "./training-type";

export interface GroupTraining {
    id: number;
    description: string;
    date: string;
    duration: string;
    statuses: string[];
    trainer: Trainer;
    maxParticipantNumber: number;
    currentParticipantNumber: number;
    trainingType: TrainingType;
    difficultyLevel: number;
}
