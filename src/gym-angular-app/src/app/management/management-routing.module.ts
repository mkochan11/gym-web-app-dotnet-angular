import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManagementLayoutComponent } from '../layouts/management-layout/management-layout.component';

const routes: Routes = [
  {
    path: '',
    component: ManagementLayoutComponent,
    children: [      
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManagementRoutingModule {}