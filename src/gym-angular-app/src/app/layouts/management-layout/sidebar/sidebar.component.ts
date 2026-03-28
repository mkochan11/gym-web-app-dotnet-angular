import { Component, ViewEncapsulation } from '@angular/core';
import { PanelMenuModule } from 'primeng/panelmenu';
import { MenuItem } from 'primeng/api';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/api-services/';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [PanelMenuModule, RouterLink],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class SidebarComponent {

  constructor(private authService: AuthService) {}

  managerSidebarItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/management/manager'] },
    { label: 'Calendar', icon: 'pi pi-calendar', routerLink: ['/management/manager/calendar'] },
    { label: 'Clients', icon: 'pi pi-users', routerLink: ['/management/manager/clients'] },
    { label: 'Membership Plans', icon: 'pi pi-credit-card', routerLink: ['/management/manager/membership-plans'] },
    { label: 'Employees', icon: 'pi pi-users', items: [
      { label: 'Trainers', icon: 'pi pi-user-edit', routerLink: ['/management/manager/trainers'] },
      { label: 'Receptionists', icon: 'pi pi-user-edit', routerLink: ['/management/manager/receptionists'] },
    ] },
    { label: 'Shifts', icon: 'pi pi-briefcase'}
  ]

  trainerSidebarItems: MenuItem[] = [
    { label: 'Trainings', icon: 'pi pi-users', items: [
      { label: 'My Trainings', icon: 'pi pi-stopwatch', routerLink: ['/management/trainer/my-trainings'] },
      { label: 'New Training', icon: 'pi pi-plus', routerLink: ['/management/trainer/all-trainings'] },
    ]},
    { label: 'Training plans', icon: 'pi pi-users', routerLink: ['/management/training-plans'] },
  ]

  adminSidebarItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/management/admin'] },
    { label: 'Users', icon: 'pi pi-users', routerLink: ['/management/admin/users'] },
    { label: 'Membership Plans', icon: 'pi pi-credit-card', routerLink: ['/management/admin/membership-plans'] },
    { label: 'Equipment', icon: 'pi pi-box', routerLink: ['/management/equipment'] },
    { label: 'Reports', icon: 'pi pi-chart-line', routerLink: ['/management/reports'] },
    { label: 'Settings', icon: 'pi pi-cog', routerLink: ['/management/settings'] },
    { label: 'Manager Panel', icon: 'pi pi-fw pi-home', routerLink: ['/management/manager'] },
  ]

  ownerSidebarItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/management/admin'] },
    { label: 'Users', icon: 'pi pi-users', routerLink: ['/management/admin/users'] },
    { label: 'Equipment', icon: 'pi pi-box', routerLink: ['/management/equipment'] },
    { label: 'Reports', icon: 'pi pi-chart-line', routerLink: ['/management/reports'] },
  ]

  receptionistSidebarItems: MenuItem[] = [
    { label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/management/receptionist'] },
    { label: 'Calendar', icon: 'pi pi-calendar', routerLink: ['/management/receptionist/calendar'] },
    { label: 'Clients', icon: 'pi pi-users', routerLink: ['/management/receptionist/clients'] },
    { label: 'Membership Plans', icon: 'pi pi-credit-card', routerLink: ['/management/receptionist/membership-plans'] },
    { label: 'Shifts', icon: 'pi pi-briefcase'}
  ]

  get model(): MenuItem[] {
    const role = this.authService.getRole();
    if (!role) return [];
    
    const roles = role.split(',').map(r => r.trim().toLowerCase());
    
    if (roles.includes('admin') || roles.includes('owner')) {
      return roles.includes('admin') ? this.adminSidebarItems : this.ownerSidebarItems;
    }
    if (roles.includes('manager')) return this.managerSidebarItems;
    if (roles.includes('trainer')) return this.trainerSidebarItems;
    if (roles.includes('receptionist')) return this.receptionistSidebarItems;
    
    return [];
  }
}
