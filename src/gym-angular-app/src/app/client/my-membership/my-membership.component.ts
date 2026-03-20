import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { SkeletonModule } from 'primeng/skeleton';
import { ButtonModule } from 'primeng/button';
import { MembershipDetailComponent } from '../../shared/components/membership/membership-detail/membership-detail.component';
import { AuthService, ClientService } from '../../core/api-services';
import { Router } from '@angular/router';

type LoadState = 'loading' | 'no_account' | 'no_client' | 'no_membership' | 'loaded';

@Component({
  selector: 'app-my-membership',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    SkeletonModule,
    ButtonModule,
    MembershipDetailComponent
  ],
  template: `
    <div class="container mx-auto p-4">
      <div class="mb-4">
        <h1 class="text-3xl font-bold text-900 mb-2">My Membership</h1>
        <p class="text-500">View and manage your gym membership</p>
      </div>

      <div *ngIf="state() === 'loading'" class="surface-card p-4 border-round shadow-2">
        <p-skeleton height="2rem" styleClass="mb-3"></p-skeleton>
        <p-skeleton height="1rem" width="60%" styleClass="mb-2"></p-skeleton>
        <p-skeleton height="1rem" width="40%"></p-skeleton>
      </div>

      <app-membership-detail 
        *ngIf="state() === 'loaded' && clientId()"
        [clientId]="clientId()!"
        (membershipCancelled)="onMembershipCancelled()">
      </app-membership-detail>

      <div *ngIf="state() === 'no_account'" class="surface-card p-5 border-round shadow-2 text-center">
        <i class="pi pi-lock text-5xl text-300 mb-4"></i>
        <h3 class="text-xl font-semibold mb-2">Please log in</h3>
        <p class="text-500 mb-4">You need to be logged in to view your membership</p>
        <p-button label="Go to Login" icon="pi pi-sign-in" routerLink="/login"></p-button>
      </div>

      <div *ngIf="state() === 'no_client'" class="surface-card p-5 border-round shadow-2 text-center">
        <i class="pi pi-user-minus text-5xl text-300 mb-4"></i>
        <h3 class="text-xl font-semibold mb-2">Account not found</h3>
        <p class="text-500 mb-4">Your account is not registered as a gym client. Please contact reception.</p>
      </div>

      <div *ngIf="state() === 'no_membership'" class="surface-card p-5 border-round shadow-2 text-center">
        <i class="pi pi-credit-card text-5xl text-300 mb-4"></i>
        <h3 class="text-xl font-semibold mb-2">No active membership</h3>
        <p class="text-500 mb-4">You don't have an active gym membership yet.</p>
        <p-button label="Purchase Membership" icon="pi pi-plus" routerLink="/membership"></p-button>
      </div>
    </div>
  `
})
export class MyMembershipComponent implements OnInit {
  private authService = inject(AuthService);
  private clientService = inject(ClientService);
  private router = inject(Router);
  
  clientId = signal<number | null>(null);
  state = signal<LoadState>('loading');

  ngOnInit() {
    this.loadClientAndMembership();
  }

  private loadClientAndMembership() {
    this.state.set('loading');
    const accountId = this.authService.getUserId();
    
    if (!accountId) {
      console.warn('[MyMembership] No account ID found - user may not be logged in');
      this.state.set('no_account');
      return;
    }

    console.log('[MyMembership] Loading client for account:', accountId);

    this.clientService.getClientByAccountId(accountId).subscribe({
      next: (client) => {
        if (!client) {
          console.warn('[MyMembership] Client not found for account:', accountId);
          this.state.set('no_client');
          return;
        }
        
        console.log('[MyMembership] Client found:', client);
        this.clientId.set(client.id);
        this.state.set('loaded');
      },
      error: (err) => {
        console.error('[MyMembership] Error loading client:', err);
        this.state.set('no_client');
      }
    });
  }

  onMembershipCancelled() {
    this.loadClientAndMembership();
  }
}
