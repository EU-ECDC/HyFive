"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.GloveComponent = void 0;
var core_1 = require("@angular/core");
var queryparameters_1 = require("../../constants/queryparameters");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var urls_1 = require("../../constants/urls");
var dialogueTexts_1 = require("../../constants/dialogueTexts");
var free_regular_svg_icons_1 = require("@fortawesome/free-regular-svg-icons");
var GloveComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-glove',
            templateUrl: './glove.component.html'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var GloveComponent = _classThis = /** @class */ (function () {
        function GloveComponent_1(sessionService, router, route, toastrService) {
            this.sessionService = sessionService;
            this.router = router;
            this.route = route;
            this.toastrService = toastrService;
            this.sessionIsSentToServer = false;
            this.sessionSentToServer = false;
            this.isOnline = true;
            this.faCalendar = free_regular_svg_icons_1.faCalendar;
            this.faAngleLeft = free_solid_svg_icons_1.faAngleLeft;
            this.faClipboard = free_solid_svg_icons_1.faClipboard;
            this.faCircle = free_solid_svg_icons_1.faCircle;
            this.faClock = free_solid_svg_icons_1.faClock;
            this.faAngleDown = free_solid_svg_icons_1.faAngleDown;
            this.DialogueTexts = dialogueTexts_1.DialogueTexts;
            this.Urls = urls_1.Urls;
        }
        GloveComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            this.route
                .queryParams
                .subscribe(function (params) {
                var sessionId = params[queryparameters_1.Queryparameters.SessionId] || 0;
                _this.session = _this.sessionService.getSession(sessionId);
                if (!_this.session)
                    _this.router.navigate(['']);
            });
        };
        GloveComponent_1.prototype.sessionDeletedEventHandler = function (id) {
            this.sessionService.deleteSession(id);
            this.router.navigate([urls_1.Urls.NotSentSessionsUrl]);
        };
        GloveComponent_1.prototype.navigateToRegistrationPageForGlove = function (sessionId) {
            this.router.navigate([urls_1.Urls.RegisterGloveUrl], { queryParams: { sessionId: sessionId } });
        };
        GloveComponent_1.prototype.observationDeletedEventHandler = function ($event) {
            this.session = this.sessionService.getSession(this.session.id);
        };
        GloveComponent_1.prototype.navigateToSentSessions = function () {
            this.router.navigate([urls_1.Urls.SentSessionsUrl]);
        };
        GloveComponent_1.prototype.showIndications = function (item) {
            if (item.gloveWithIndicationTypes.length)
                return item.gloveWithIndicationTypes.map(function (x) { return x.name; }).join(', ');
            ;
            return item.gloveWithoutIndicationTypes.map(function (x) { return x.name; }).join(', ');
        };
        GloveComponent_1.prototype.sendToCoordinator = function () {
            var _this = this;
            this.sessionSentToServer = true;
            this.sessionService.sendToServer(this.session.id).subscribe(function (res) {
                _this.toastrService.success("Session was sent to coordinator");
                _this.sessionService.deleteSession(_this.session.id);
                _this.sessionIsSentToServer = true;
            }, function (error) {
                var message = "Something went wrong while sending session to coordinator: " + ((error === null || error === void 0 ? void 0 : error.error) ? error.error.substr(0, 300) + '...' : error);
                _this.toastrService.error(message, '', { disableTimeOut: true });
            }, function () { return _this.sessionSentToServer = false; });
        };
        ;
        GloveComponent_1.prototype.navigateToSentSession = function () {
            this.router.navigate(['/' + urls_1.Urls.SentGloveSessionUrl], { queryParams: { sessionId: this.session.id } });
        };
        return GloveComponent_1;
    }());
    __setFunctionName(_classThis, "GloveComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        GloveComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return GloveComponent = _classThis;
}();
exports.GloveComponent = GloveComponent;
//# sourceMappingURL=glove.component.js.map