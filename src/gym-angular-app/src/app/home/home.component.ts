import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <main class="hero">
      <div class="hero-inner">
        <h1>Welcome to Gym Web App</h1>
        <p>Plan workouts, track progress and get coached — all in one place. Sign in to access your trainings and management dashboard.</p>
        <a class="cta" routerLink="/login">Get started</a>
        <a class="cta secondary" href="/#features" style="margin-left:12px">Learn more</a>
      </div>
    </main>
  `
})
export class HomeComponent {
  year = new Date().getFullYear();
}
