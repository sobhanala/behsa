import {Directive, ElementRef, EventEmitter, HostListener, Output} from '@angular/core';

@Directive({
  selector: '[appBlurClick]',
  standalone: true
})
export class BlurClickDirective {
  @Output() blurClick = new EventEmitter<void>();

  constructor(private element: ElementRef) {}

  @HostListener('document:click', ['$event.target'])
  handleBlur(element: HTMLElement) {
    if (this.element.nativeElement.style.display === 'flex' && !this.element.nativeElement.contains(element)
      && element.dataset['triggerButton'] === undefined) {
      this.blurClick.emit();
    }
  }
}
