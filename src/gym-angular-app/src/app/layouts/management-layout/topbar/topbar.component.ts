import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { UserMenuComponent } from "../user-menu/user-menu.component";
import { primeNgModules } from '../../../shared/primeng';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/internal/operators/filter';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [ButtonModule, UserMenuComponent, ...primeNgModules],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit{
  pageTitle: string = 'Dashboard';
  panelTitle: string = 'Manager panel';

  constructor(private router: Router) {}

  ngOnInit() {
    this.updatePageTitle(this.router.url);
    this.updatePanelTitle(this.router.url);

    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd)
      )
      .subscribe((event: NavigationEnd) => {
        this.updatePageTitle(event.url);
        this.updatePanelTitle(event.url);
      });
  }

  private updatePageTitle(url: string): void {
    if (url.includes('/management/manager/trainers')) {
      this.pageTitle = 'Manage Trainers';
    } else if (url.includes('/management/manager/receptionists')) {
      this.pageTitle = 'Manage Receptionists';
    } else if (url.includes('/management/manager/clients')) {
      this.pageTitle = 'Manage Clients';
    } else if (url.includes('/management/manager')) {
      this.pageTitle = 'Dashboard';
    } else {
      this.pageTitle = 'Dashboard';
    }
  }

  private updatePanelTitle(url: string): void {
    if (url.includes('/management/admin')) {
      this.panelTitle = 'Admin Panel';
    } else if (url.includes('/management/owner')) {
      this.panelTitle = 'Owner Panel';
    } else if (url.includes('/management/manager')) {
      this.panelTitle = 'Manager Panel';
    } else if (url.includes('/management/trainer')) {
      this.panelTitle = 'Trainer Panel';
    } else if (url.includes('/management/receptionist')) {
      this.panelTitle = 'Receptionist Panel';
    }
  }
}
