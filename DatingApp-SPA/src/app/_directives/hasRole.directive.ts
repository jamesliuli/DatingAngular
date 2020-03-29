import { Directive, Input, ViewContainerRef, TemplateRef, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  isVisible = false;

  constructor(private viewContainerRef: ViewContainerRef,
              private templateRef: TemplateRef<any>,
              private authService: AuthService) {

  }

  ngOnInit() {

    this.authService.loginUserChanged.subscribe(() => {
      this.ShowAdminPanel();
    });
  }

  ShowAdminPanel() {
      if (!this.authService.decodedToken) {
          this.viewContainerRef.clear();
          this.isVisible = false;
          console.log('hide admin');
          return;
      }

      const userRoles = this.authService.decodedToken.role as Array<string>;
      if (!userRoles) {
        this.isVisible = false;
        this.viewContainerRef.clear();
        console.log('hide admin');
        return;
      }

      if (this.authService.isRoleMatch(this.appHasRole)) {
        if (!this.isVisible) {
          this.isVisible = true;
          this.viewContainerRef.createEmbeddedView(this.templateRef);
          console.log('show admin');
        }} else {
        this.isVisible = false;
        this.viewContainerRef.clear();
        console.log('hide admin');
      }
  }
}
