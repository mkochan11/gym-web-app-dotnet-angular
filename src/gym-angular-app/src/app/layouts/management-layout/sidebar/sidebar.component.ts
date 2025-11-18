import { Component, ViewEncapsulation } from '@angular/core';
import { PanelMenuModule } from 'primeng/panelmenu';
import { MenuItem } from 'primeng/api';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../shared/services';

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
    { label: 'Users', icon: 'pi pi-users', routerLink: ['/management/users'] },
    { label: 'Equipment', icon: 'pi pi-box', routerLink: ['/management/equipment'] },
    { label: 'Reports', icon: 'pi pi-chart-line', routerLink: ['/management/reports'] },
    { label: 'Settings', icon: 'pi pi-cog', routerLink: ['/management/settings'] }
  ]

  ownerSidebarItems: MenuItem[] = [
    { label: 'Users', icon: 'pi pi-users', routerLink: ['/management/users'] },
    { label: 'Equipment', icon: 'pi pi-box', routerLink: ['/management/equipment'] },
    { label: 'Reports', icon: 'pi pi-chart-line', routerLink: ['/management/reports'] },
  ]

  get model(): MenuItem[] {
    return this.authService.getRole() === 'Manager' ? this.managerSidebarItems :
           this.authService.getRole() === 'Trainer' ? this.trainerSidebarItems :
           this.authService.getRole() === 'Admin' ? this.adminSidebarItems :
           this.authService.getRole() === 'Owner' ? this.ownerSidebarItems : [];
  }
}
