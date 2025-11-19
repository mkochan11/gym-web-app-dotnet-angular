import { Component } from '@angular/core';
import { CommonModule, NgIf, AsyncPipe } from '@angular/common';
import { ToastService, ToastMessage } from '../../services';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule, NgIf, AsyncPipe],
  templateUrl: './toast.component.html',
  styleUrls: ['./toast.component.scss'],
  animations: [
    trigger('toastAnimation', [
      transition(':enter', [
        style({ 
          opacity: 0, 
          transform: 'translate(-50%, -50%) scale(0.8)'
        }),
        animate('300ms ease-out', style({ 
          opacity: 1, 
          transform: 'translate(-50%, -50%) scale(1)'
        }))
      ]),
      transition(':leave', [
        animate('300ms ease-in', style({ 
          opacity: 0, 
          transform: 'translate(-50%, -50%) scale(0.8)'
        }))
      ])
    ])
  ]
})
export class ToastComponent {
  toast$;

  constructor(private toastService: ToastService) {
    this.toast$ = this.toastService.toast$;
  }
}