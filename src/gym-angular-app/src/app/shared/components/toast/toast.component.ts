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
        style({ opacity: 0, transform: 'translateY(20px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ]),
      transition(':leave', [
        animate('300ms ease-in', style({ opacity: 0, transform: 'translateY(20px)' }))
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
