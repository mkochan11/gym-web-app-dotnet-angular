import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-receptionist-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class ReceptionistDashboardComponent {
  
  constructor(private router: Router) {}

  isBaseRoute(): boolean {
    return this.router.url === '/management/receptionist';
  }
}
