import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { SidebarComponent } from './sidebar/sidebar.component';
import { TopbarComponent } from './topbar/topbar.component';
import { AuthService } from '../../core/api-services';

@Component({
  selector: 'app-management-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent],
  templateUrl: './management-layout.component.html',
  styleUrls: ['./management-layout.component.scss']
})
export class ManagementLayoutComponent implements OnInit {
  private auth = inject(AuthService);
  private router = inject(Router);

  ngOnInit(){
    const currentUrl = this.router.url;
    if (currentUrl === '/management' || currentUrl === '/management/') {
      const role = this.auth.getRole();
      if (role) {
        const roles = role.split(',').map(r => r.trim().toLowerCase());
        
        let targetRole = '';
        if (roles.includes('admin')) targetRole = 'admin';
        else if (roles.includes('owner')) targetRole = 'admin';
        else if (roles.includes('manager')) targetRole = 'manager';
        else if (roles.includes('trainer')) targetRole = 'trainer';
        else if (roles.includes('receptionist')) targetRole = 'receptionist';
        
        if (targetRole) {
          this.router.navigate([`/management/${targetRole}`]);
        }
      }
    }
  }
}
