import {Component, ElementRef, EventEmitter, HostListener, Output, ViewChild} from '@angular/core';
import {UserService} from "../../../services/user/user.service";
import User from "../../../interfaces/user";
import {FormsModule} from "@angular/forms";
import {HttpClient} from "@angular/common/http";
import {API_BASE_URL} from "../../../app.config";
import {RialPipePipe} from "./pipes/rial-pipe.pipe";
import {PersianDatePipe} from "./pipes/persian-date.pipe";
import {heroXMark} from "@ng-icons/heroicons/outline";
import {NgIconComponent, provideIcons} from "@ng-icons/core";
import {BlurClickDirective} from "../../../directives/blur-click.directive";
import * as d3 from 'd3';
import {FetchDataService} from "../../../services/fetchData/fetch-data.service";
import {Account, Link, Transaction, Node} from "../../../interfaces/other";

@Component({
  selector: 'app-show-data',
  standalone: true,
  imports: [
    FormsModule,
    RialPipePipe,
    PersianDatePipe,
    NgIconComponent,
    BlurClickDirective
  ],
  templateUrl: './show-data.component.html',
  styleUrl: './show-data.component.scss',
  providers: [provideIcons({heroXMark})]
})
export class ShowDataComponent {
  user!: User | undefined;
  data: Transaction[] | undefined = undefined;
  nodes: Node[] = [];
  links: Link[] = [];
  account: Account | undefined = undefined;
  graphRendered = false;

  element!: d3.Selection<SVGSVGElement, unknown, null, undefined>;
  svgGroup!: d3.Selection<SVGGElement, unknown, null, undefined>;
  simulation!: d3.Simulation<Node, Link>;
  link!: d3.Selection<SVGLineElement, Link, SVGGElement, unknown>;
  linkLabelsAmount!: d3.Selection<SVGTextElement, Link, SVGGElement, unknown>;
  linkLabelsDate!: d3.Selection<SVGTextElement, Link, SVGGElement, unknown>;
  linkLabelsType!: d3.Selection<SVGTextElement, Link, SVGGElement, unknown>;
  node!: d3.Selection<SVGCircleElement, Node, SVGGElement, unknown>;
  nodeLabels!: d3.Selection<SVGTextElement, Node, SVGGElement, unknown>;

  @Output() dataGotEvent = new EventEmitter<string>();

  @ViewChild('labelElement') labelElement!: ElementRef<HTMLLabelElement>;
  @ViewChild('inputElement') inputElement!: ElementRef<HTMLInputElement>;
  @ViewChild('selectElement') selectElement!: ElementRef<HTMLSelectElement>;
  @ViewChild('dataElement') dataElement!: ElementRef<HTMLDivElement>;
  @ViewChild('graphElement') graphElement!: ElementRef<HTMLDivElement>;
  @ViewChild('contextElement') contextElement!: ElementRef<HTMLDivElement>;
  @ViewChild('searchIdElement') searchIdElement!: ElementRef<HTMLInputElement>;
  @ViewChild('userContainer') userElement!: ElementRef<HTMLDivElement>;

  constructor(private userService: UserService, private http: HttpClient, private fetchDataService: FetchDataService) {
    this.user = this.userService.getUser();
  }

  handleChange(): void {
    if (this?.inputElement?.nativeElement?.files && this?.inputElement?.nativeElement?.files?.length > 0) {
      this.labelElement.nativeElement.textContent = this?.inputElement?.nativeElement?.files[0].name;
    }
  }

  handleDisabled(): boolean {
    return !(this?.inputElement?.nativeElement?.files && this?.inputElement?.nativeElement?.files?.length > 0);
  }

  clearGraphTable(): void {
    d3.select(this.graphElement.nativeElement).selectAll('*').remove();
  }

  async handleGetUser() {
    this.nodes = [];
    this.links = [];
    this.clearGraphTable();
    if (this.searchIdElement.nativeElement.value === '') {
      await this.getAllData();
      return;
    }
    const response = await this.fetchDataService.fetchDataById(this.searchIdElement.nativeElement.value);
    if (response.length === 0) {
      this.graphElement.nativeElement.textContent = "داده ای یافت نشد!";
      return;
    }
    this.nodes.push({
      x: 1,
      y: 1,
      vx: 1,
      vy: 1,
      label: Number(this.searchIdElement.nativeElement.value)
    });
    for (const item of response) {
      if (!this.nodes.find(node => node.label === item.accountId)) {
        this.nodes.push({
          x: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].x + 1 : 1,
          y: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].y + 1 : 1,
          vx: 1,
          vy: 1,
          label: item.accountId,
        });
      }
      this.links.push({
        source: item.transactionWithSources[0].sourceAcount === item.accountId ?
          this.nodes.find(node => node.label === item.accountId)!
          : this.nodes.find(node => node.label === Number(this.searchIdElement.nativeElement.value))!,
        target: item.transactionWithSources[0].sourceAcount === item.accountId ?
          this.nodes.find(node => node.label === Number(this.searchIdElement.nativeElement.value))!
          : this.nodes.find(node => node.label === item.accountId)!,
        type: item.transactionWithSources[0].type,
        amount: (new RialPipePipe()).transform(item.transactionWithSources[0].amount),
        date: (new PersianDatePipe()).transform(item.transactionWithSources[0].date),
      })
    }
    this.searchIdElement.nativeElement.value = "";
    this.dataGotEvent.emit("none-all");
  }

  onSubmit(): void {
    const formData: FormData = new FormData();
    const token: string | null = this.getToken();
    if (this.inputElement.nativeElement.files) {
      const file = this.inputElement.nativeElement.files[0];
      formData.append('file', file);
      if (this.selectElement.nativeElement.value === "transaction") {
        this.http.post(API_BASE_URL + 'transactions/upload', formData, {headers: {"Authorization": "Bearer " + token}}).subscribe((response) => {
          console.log(response);
        })
      } else if (this.selectElement.nativeElement.value === "account") {
        this.http.post(API_BASE_URL + 'accounts/upload', formData, {headers: {"Authorization": "Bearer " + token}}).subscribe((response) => {
          console.log(response);
        })
      }
    }
  }

  showData(): void {
    this.dataElement.nativeElement.style.display = 'flex';
  }

  handleClose(): void {
    this.dataElement.nativeElement.style.display = 'none';
  }

  getToken(): string | null {
    let token = localStorage.getItem("token");
    if (token) {
      token = token.substring(1, token.length - 1);
    }

    return token;
  }

  async ngOnInit() {
    await this.getAllData();
  }

  async getAllData(): Promise<void> {
    const response = await this.fetchDataService.fetchData();
    this.data = response;
    for (const trans of response) {
      if (!this.nodes.find(node => node.label === trans.sourceAccountId)) {
        this.nodes.push({
          x: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].x + 1 : 1,
          y: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].y + 1 : 1,
          vx: 1,
          vy: 1,
          label: trans.sourceAccountId,
        });
      }
      if (!this.nodes.find(node => node.label === trans.destinationAccountId)) {
        this.nodes.push({
          x: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].x + 1 : 1,
          y: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].y + 1 : 1,
          vx: 1,
          vy: 1,
          label: trans.destinationAccountId,
        });
      }
      this.links.push({
        source: this.nodes.find(node => node.label === trans.sourceAccountId)!,
        target: this.nodes.find(node => node.label === trans.destinationAccountId)!,
        date: (new PersianDatePipe()).transform(trans.date),
        type: trans.type,
        amount: (new RialPipePipe()).transform(trans.amount),
      });
    }
    this.dataGotEvent.emit("all");
  }

  @HostListener('dataGotEvent', ['$event'])
  handleGraph(callType: string): void {
    this.graphRendered = false;
    this.graphElement.nativeElement.textContent = "";
    this.element = d3.select(this.graphElement.nativeElement)
      .append('svg')
      .attr('width', this.graphElement.nativeElement.clientWidth)
      .attr('height', this.graphElement.nativeElement.clientHeight);

    d3.select(this.graphElement.nativeElement)
      .append('pre')
      .text("برای انحام اعمال خاص بر روی هر راس راست کلیک کنید.\n" +
        "برای گسترش گراف در حالت جستجو شده، باید تا اتمام ساخت گراف\n (رندر و ری-رندر شدن آن) صبر کنید.")
      .attr("style", "" +
        "position: absolute;" +
        "inset-inline-start: 0.25rem;" +
        "inset-block-start: 0.25rem;" +
        "color: #172535;" +
        "z-index: 10;" +
        "font-size: 1.8rem;" +
        "opacity: 0.7;");

    this.svgGroup = this.element.append('g');

    const zoom = d3.zoom<SVGSVGElement, unknown>()
      .scaleExtent([0.5, 4])
      .on('zoom', (event) => {
        this.svgGroup.attr('transform', event.transform);
      });

    this.element.call(zoom);

    this.simulation = d3.forceSimulation(this.nodes)
      .force("link", d3.forceLink(this.links));

    this.svgGroup.append('defs').append('marker')
      .attr('id', 'arrowhead')
      .attr('viewBox', '-0 -5 10 10')
      .attr('refX', 13)
      .attr('refY', 0)
      .attr('orient', 'auto')
      .attr('markerWidth', 12)
      .attr('markerHeight', 12)
      .attr('xoverflow', 'visible')
      .append('svg:path')
      .attr('d', 'M 0,-5 L 10 ,0 L 0,5')
      .attr('fill', '#FDFDFD')
      .style('stroke', 'none');

    this.link = this.svgGroup.append('g')
      .attr('class', 'links')
      .selectAll('line')
      .data(this.links)
      .enter()
      .append('line')
      .attr('stroke-width', 2)
      .attr('stroke', '#FDFDFD')
      .attr('marker-end', 'url(#arrowhead)');


    this.linkLabelsAmount = this.svgGroup.append('g')
      .attr('class', 'link-labels')
      .selectAll('text')
      .data(this.links)
      .enter()
      .append('text')
      .attr('text-anchor', 'middle')
      .attr('fill', '#172535')
      .attr('style', 'user-select: none;font-weight:bold;font-size:1.5rem;')
      .text((d: Link) => d.amount ? d.amount : "");

    this.linkLabelsType = this.svgGroup.append('g')
      .attr('class', 'link-labels')
      .selectAll('text')
      .data(this.links)
      .enter()
      .append('text')
      .attr('text-anchor', 'middle')
      .attr('fill', '#172535')
      .attr('style', 'user-select: none;font-weight:bold;font-size:1.5rem;')
      .text((d: Link) => d.type ? d.type : "");

    this.linkLabelsDate = this.svgGroup.append('g')
      .attr('class', 'link-labels')
      .selectAll('text')
      .data(this.links)
      .enter()
      .append('text')
      .attr('text-anchor', 'middle')
      .attr('fill', '#172535')
      .attr('style', 'user-select: none;font-weight:bold;font-size:1.5rem;')
      .text((d: Link) => d.date ? d.date : "");

    this.node = this.svgGroup.append('g')
      .attr('class', 'nodes')
      .selectAll('circle')
      .data(this.nodes)
      .enter()
      .append('circle')
      .attr('r', 10)
      .attr('fill', '#002B5B')
      .call(d3.drag<SVGCircleElement, Node>()
        .on('start', (event: d3.D3DragEvent<SVGCircleElement, Node, Node>, d: Node) => {
          if (!event.active) this.simulation.alphaTarget(0.3).restart();
          this.graphRendered = false;
          d.fx = d.x;
          d.fy = d.y;
        })
        .on('drag', (event: d3.D3DragEvent<SVGCircleElement, Node, Node>, d: Node) => {
          d.fx = event.x;
          d.fy = event.y;
        })
        .on('end', (event: d3.D3DragEvent<SVGCircleElement, Node, Node>, d: Node) => {
          if (!event.active) this.simulation.alphaTarget(0);
          d.fx = null;
          d.fy = null;
        })
      );

    this.node.on('contextmenu', (event: MouseEvent, d: Node) => {
      event.preventDefault();

      const token = this.getToken();

      this.http.get<Account>(API_BASE_URL + `accounts/${d.label}`, {headers: {'Authorization': "Bearer " + token}})
        .subscribe((res) => {
        this.account = res;
      });

      this.contextElement.nativeElement.style.display = 'flex';
      this.contextElement.nativeElement.style.top = event.clientY + 'px';
      this.contextElement.nativeElement.style.left = event.clientX + 'px';

      const expandButton = this.contextElement.nativeElement.querySelector('ul > li:last-child');
      expandButton?.classList.add("disabled");
      if (callType === "all") {
        expandButton?.setAttribute("style", "display: none;");
      } else {
        expandButton?.setAttribute("style", "display: block;");
      }
    });

    this.nodeLabels = this.svgGroup.append('g')
      .attr('class', 'node-labels')
      .selectAll('text')
      .data(this.nodes)
      .enter()
      .append('text')
      .attr('text-anchor', 'middle')
      .attr('dy', -10) // Position above the node
      .attr('fill', '#172535')
      .attr('style', 'user-select: none;font-weight:bold;font-size:1.5rem;')
      .text((d: Node) => d.label ? d.label : "");

    this.simulation.on('tick', () => {
      this.link
        .attr('x1', d => d.source.x)
        .attr('y1', d => d.source.y)
        .attr('x2', d => d.target.x)
        .attr('y2', d => d.target.y);

      this.node
        .attr('cx', d => d.x)
        .attr('cy', d => d.y);

      // Update positions of node labels
      this.nodeLabels
        .attr('x', d => d.x)
        .attr('y', d => d.y);

      // Update positions of link labels
      this.linkLabelsAmount
        .attr('x', d => ((d.source as Node).x + (d.target as Node).x) / 2)
        .attr('y', d => ((d.source as Node).y + (d.target as Node).y) / 2);

      this.linkLabelsDate
        .attr('x', d => ((d.source as Node).x + (d.target as Node).x) / 2)
        .attr('y', d => ((d.source as Node).y + (d.target as Node).y) / 2 + 20);

      this.linkLabelsType
        .attr('x', d => ((d.source as Node).x + (d.target as Node).x) / 2)
        .attr('y', d => ((d.source as Node).y + (d.target as Node).y) / 2 + 40);
    });

    this.simulation
      .force('link', d3.forceLink(this.links).id((d, i) => i).distance(250))
      .force('charge', d3.forceManyBody().strength(-350))
      .force('center', d3.forceCenter(this.graphElement.nativeElement.clientWidth / 2, this.graphElement.nativeElement.clientHeight / 2))
      .on("end", () => {
        this.graphRendered = true;
        const expandButton = this.contextElement.nativeElement.querySelector('ul > li:last-child');
        expandButton?.classList.remove("disabled");
      });

  }

  handleCloseContext() {
    this.contextElement.nativeElement.style.display = 'none';
  }

  handleShowUser() {
    this.userElement.nativeElement.style.display = 'flex';
  }

  handleCloseUser() {
    this.userElement.nativeElement.style.display = 'none';
  }

  async handleExpandGraph(): Promise<void> {
    if (!this.graphRendered) return;
    const newData = await this.fetchDataService.fetchDataById(String(this.account?.accountId));
    this.clearGraphTable();
    for (const item of newData) {
      if (!this.nodes.find(node => node.label === item.accountId)) {
        this.nodes.push({
          index: this.nodes.length,
          x: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].x + 1 : 1,
          y: this.nodes[this.nodes.length - 1] ? this.nodes[this.nodes.length - 1].y + 1 : 1,
          vx: 1,
          vy: 1,
          label: item.accountId,
        });
      }
      if (!this.links.find(link => link.source.label === item.accountId && link.target.label === this.account?.accountId) &&
          !this.links.find(link => link.source.label === this.account?.accountId && link.target.label === item.accountId)) {
        this.links.push({
          index: this.links.length,
          source: item.transactionWithSources[0].sourceAcount === item.accountId ?
            this.nodes.find(node => node.label === item.accountId)!
            : this.nodes.find(node => node.label === this.account?.accountId)!,
          target: item.transactionWithSources[0].sourceAcount === item.accountId ?
            this.nodes.find(node => node.label === this.account?.accountId)!
            : this.nodes.find(node => node.label === item.accountId)!,
          type: item.transactionWithSources[0].type,
          amount: (new RialPipePipe()).transform(item.transactionWithSources[0].amount),
          date: (new PersianDatePipe()).transform(item.transactionWithSources[0].date),
        });
      }
    }
    this.dataGotEvent.emit("none-all");
  }
}
