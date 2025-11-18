import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { SidebarComponent } from './sidebar/sidebar.component';
import { TopbarComponent } from './topbar/topbar.component';
import { AuthService } from '../../shared/services';

@Component({
  selector: 'app-management-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent],
  templateUrl: './management-layout.component.html',
  styleUrls: ['./management-layout.component.scss']
})
export class ManagementLayoutComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  ngOnInit(){
    const role = this.auth.getRole()?.toLowerCase();
    if (role) {
      this.router.navigate([`/management/${role}`]);
    }
  }
}
