import { Component, OnDestroy, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { navItems } from '../../_nav';
import { AuthService } from '../../shared/auth.service';
import { Router } from '@angular/router';
import { Service } from '../../core/api.client';


@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html'
})
export class DefaultLayoutComponent implements OnDestroy {
  public navItems = navItems;
  public sidebarMinimized = true;
  private changes: MutationObserver;
  public element: HTMLElement;
  fullName: '';
  constructor(
    private router: Router,
    public authService: AuthService,
    private apiService: Service,
    @Inject(DOCUMENT) _document?: any) {

    this.changes = new MutationObserver((mutations) => {
      this.sidebarMinimized = _document.body.classList.contains('sidebar-minimized');
    });
    this.element = _document.body;
    this.changes.observe(<Element>this.element, {
      attributes: true,
      attributeFilter: ['class']
    });
  }

  logOut() {
    this.apiService.logout(this.authService.getCurrentUser().auth_token).subscribe(res => {
      this.router.navigate(['login']);
      this.authService.logout();
    });
  }

  ngOnInit() {
    this.checkIsLoggedIn();
    this.fullName = this.authService.getCurrentUser().branchName;
  }

  checkIsLoggedIn() {
    this.apiService.validateIsLoggedIn(this.authService.getToken()).subscribe(res => {
      if (res.result === 'Success') {
        this.authService.logout();
        this.router.navigate(['login']);
      }
    });
  }

  ngOnDestroy(): void {
    this.changes.disconnect();
  }
}
