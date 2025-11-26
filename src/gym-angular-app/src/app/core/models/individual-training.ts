import { Client } from "./client";
import { Trainer } from "./trainer";

export interface IndividualTraining {
    id: number;
    description: string;
    date: string;
    duration: string;
    isCompleted: boolean;
    isCancelled: boolean;
    trainer: Trainer;
    client: Client;
}
