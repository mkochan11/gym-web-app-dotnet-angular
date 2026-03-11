import { Client } from "./client";
import { Trainer } from "./trainer";

export interface IndividualTraining {
    id: number;
    description: string;
    date: string;
    duration: string;
    statuses: string[];
    trainer: Trainer;
    client: Client;
}
