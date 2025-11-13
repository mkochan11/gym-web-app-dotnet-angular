import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  hideLayout = false;
  year = new Date().getFullYear();

  constructor(private router: Router) {
    router.events.subscribe(() => {
      const url = this.router.url;
      this.hideLayout = url.startsWith('/login') || url.startsWith('/register');
    });
  }
}
