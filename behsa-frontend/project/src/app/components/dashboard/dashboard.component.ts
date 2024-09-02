import {Component, ElementRef, HostListener, Renderer2, ViewChild, PLATFORM_ID, Inject} from '@angular/core';
import {NavbarComponent} from "./navbar/navbar.component";
import {RouterOutlet} from "@angular/router";
import {NgIconComponent, provideIcons} from "@ng-icons/core";
import { heroBars3, heroArrowUturnRight } from '@ng-icons/heroicons/outline'
import {isPlatformBrowser, NgClass} from "@angular/common";
import { BgGifComponent } from "../bg-gif/bg-gif.component";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    NavbarComponent,
    RouterOutlet,
    NgIconComponent,
    NgClass,
    BgGifComponent
],
  providers: [provideIcons({heroBars3, heroArrowUturnRight})],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  navbarOpen: boolean = false;
  isBrowser!: boolean;
  screenWidth!: number;
  @ViewChild('navbar') navbarElement!: ElementRef<HTMLElement>;
  @ViewChild('content') contentRef!: ElementRef<HTMLElement>;
  @ViewChild('navbarIcon') barsRef!: ElementRef<HTMLElement>;

constructor(private renderer: Renderer2, @Inject(PLATFORM_ID) private platform: object) {
  this.isBrowser = isPlatformBrowser(this.platform);
}

  ngOnInit() {
    if(this.isBrowser) {
      this.screenWidth = window.innerWidth;
    }
  }

  @HostListener('window:resize', ['$event'])
  resizeWidth() {
    if (this.isBrowser) {
      this.screenWidth = window.innerWidth;
    }
    if (this.navbarOpen) {
      if (this.screenWidth >= 1024) {
        this.renderer.setStyle(this.contentRef.nativeElement, 'display', 'flex');
        this.renderer.setStyle(this.contentRef.nativeElement, 'inline-size', '80vw');
      } else {
        this.renderer.setStyle(this.contentRef.nativeElement, 'display', 'none');
      }
    }
  }

  handleShowNavbar() {
      this.navbarOpen = true;
      this.renderer.removeClass(this.navbarElement.nativeElement, 'hidden')
      this.renderer.addClass(this.navbarElement.nativeElement, 'block');
      this.renderer.removeClass(this.barsRef.nativeElement, 'block')
      this.renderer.addClass(this.barsRef.nativeElement, 'hidden');
      if (this.screenWidth >= 1024) {
        this.renderer.setStyle(this.contentRef.nativeElement, 'inline-size', '80vw');
      } else {
        this.renderer.setStyle(this.contentRef.nativeElement, 'display', 'none');
      }
  }

  handleCloseNavbar() {
    this.navbarOpen = false;
    this.renderer.removeClass(this.navbarElement.nativeElement, 'block')
    this.renderer.addClass(this.navbarElement.nativeElement, 'hidden');
    this.renderer.removeClass(this.barsRef.nativeElement, 'hidden')
    this.renderer.addClass(this.barsRef.nativeElement, 'block');
    this.renderer.setStyle(this.contentRef.nativeElement, 'inline-size', '99vw');
    this.renderer.setStyle(this.contentRef.nativeElement, 'display', 'flex');
  }

  isMenuOpen = false;

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }
}
