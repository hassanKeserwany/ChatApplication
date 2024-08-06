import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  ViewChild,
} from '@angular/core';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
import { NgForm } from '@angular/forms';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css',
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm!: NgForm;
  @ViewChild('scrollMe') scrollMe: any;

  @Input() username!: string;
  @Input() messages!: Message[];

  loading = false;
  messageContent: string = '';

  scrollTop!: number;

  constructor(
    public messageService: MessageService,
    private cdr: ChangeDetectorRef
  ) {}
  ngOnInit(): void {
    this.messageService.messageThread$.subscribe((res) => {
      this.messages = res;
    });
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      if (this.scrollMe) {
        this.scrollMe.nativeElement.scrollTop =
          this.scrollMe.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error(err);
    }
  }

  //   sendMessage() {
  //     this.messageService.sendMessage(this.username, this.messageContent).then((message) => {

  //       this.messages.push(message);
  //       this.messageForm.reset();

  //   })
  // }

  sendMessage() {
    if (!this.username) return;
    this.loading = true;
    this.messageService
      .sendMessage(this.username, this.messageContent)
      .then(() => {
        this.messageForm?.reset();
      })
      .finally(() => (this.loading = false));
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(
        this.messages.findIndex((m) => m.id === id),
        1
      );
      this.cdr.markForCheck();
    });
  }

  deleteConversation() {
    if (this.messages.length == 0) {
      console.log('no messages');
      return;
    }
    if (!this.username) {
      console.error('Username is required');
      return;
    }

    this.messageService.deleteConversationForUser(this.username).subscribe(
      () => {
        // Clear the messages on successful deletion
        this.messages = [];
        this.cdr.detectChanges(); // Force change detection
      },
      (error) => {
        // Log the error to console
        console.error('Error deleting conversation', error);
      }
    );
  }


  resizeTextArea(textarea: HTMLTextAreaElement) {
    textarea.style.height = 'auto'; // Reset height to auto
    textarea.style.height = `${textarea.scrollHeight}px`; // Set height to scrollHeight
  }
}
