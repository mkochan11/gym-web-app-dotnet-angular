import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

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

    setTimeout(() => {
      this._toast$.next(null);
    }, duration);
  }
}