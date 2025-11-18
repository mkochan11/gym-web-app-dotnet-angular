import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { primeNgModules } from '../../shared/primeng';
import { style } from '@angular/animations';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ...primeNgModules],
  templateUrl: './home-layout.component.html',
  styleUrls: ['./home-layout.component.scss']
})
export class HomeLayoutComponent {
  year = new Date().getFullYear();

  itemsRight = [
    {
      label: 'Sign in',
      routerLink: '/login',
      styleClass: 'p-button-success'
    },
    {
      label: 'Sign up',
      routerLink: '/register',
      styleClass: 'btn-black'
    }
  ];
}
