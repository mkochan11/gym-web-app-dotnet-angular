import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  
  constructor(private router: Router, private route: ActivatedRoute) {}

  isBaseRoute(): boolean {
    return this.router.url === '/management/manager';
  }
}