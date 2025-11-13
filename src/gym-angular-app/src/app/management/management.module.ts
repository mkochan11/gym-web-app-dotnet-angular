import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ManagementRoutingModule } from './management-routing.module';
import { ManagementLayoutComponent } from '../layouts/management-layout/management-layout.component'

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ManagementLayoutComponent,
    ManagementRoutingModule
  ]
})
export class ManagementModule { }
