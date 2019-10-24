import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';
import { RegisterComponent } from './views/register/register.component';

// Guards
import { AuthGuard } from './_guards/auth.guard';
import { LoginGuard } from './_guards/login.guard';

// Modules
import { PickAndPackComponent } from './views/pickandpack/pickandpack.component';
import { PicklistComponent } from './views/picklist/picklist.component';
import { OpenSOComponent } from './views/openso/openso.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'pickandpack',
    pathMatch: 'full',
  },
  {
    path: '404',
    component: P404Component,
    data: {
      title: 'Page 404'
    }
  },
  {
    path: '500',
    component: P500Component,
    data: {
      title: 'Page 500'
    }
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [LoginGuard],
    data: {
      title: 'Login Page'
    }
  },
  {
    path: 'register',
    component: RegisterComponent,
    data: {
      title: 'Register Page'
    }
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    canActivate: [AuthGuard],
    data: {
      title: 'Home'
    },
    children: [
      {
        path: 'pickandpack',
        component: PickAndPackComponent,
        data: {
          title: 'Pick and Pack'
        }
      },
      {
        path: 'picklist',
        component: PicklistComponent,
        data: {
          title: 'Picklist'
        }
      },
      {
        path: 'openso',
        component: OpenSOComponent,
        data: {
          title: 'Open SO'
        }
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./views/dashboard/dashboard.module').then(m => m.DashboardModule)
      }
    ]
  },
  { path: '**', component: P404Component }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
