import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, timer } from 'rxjs';

export interface ToastMessage {
  text: string;
  type?: 'success' | 'error' | 'info';
  duration?: number;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private _toast$ = new BehaviorSubject<ToastMessage | null>(null);
  public toast$ = this._toast$.asObservable();

  show(message: string, type: 'success' | 'error' | 'info' = 'info', duration = 3000) {
    this._toast$.next({ text: message, type, duration });

    timer(duration).subscribe(() => {
      this._toast$.next(null);
    });
  }
}