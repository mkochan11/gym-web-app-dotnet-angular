import { Client } from "./client";
import { Trainer } from "./trainer";

export interface IndividualTraining {
    id: number;
    description: string;
    date: string;
    duration: string;
    status: string;
    trainer: Trainer;
    client: Client;
}
