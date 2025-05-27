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
exports.ProtectiveEquipmentComponent = void 0;
var core_1 = require("@angular/core");
var queryparameters_1 = require("../../constants/queryparameters");
var urls_1 = require("../../constants/urls");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var free_regular_svg_icons_1 = require("@fortawesome/free-regular-svg-icons");
var dialogueTexts_1 = require("../../constants/dialogueTexts");
var ProtectiveEquipmentComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-protection-equipment',
            templateUrl: './protection-equipment.component.html'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var ProtectiveEquipmentComponent = _classThis = /** @class */ (function () {
        function ProtectiveEquipmentComponent_1(sessionService, router, route, toastrService) {
            this.sessionService = sessionService;
            this.router = router;
            this.route = route;
            this.toastrService = toastrService;
            this.sessionIsSentToServer = false;
            this.sessionSentToServer = false;
            this.isOnline = true;
            this.DialogueTexts = dialogueTexts_1.DialogueTexts;
            this.Urls = urls_1.Urls;
            this.faCircle = free_solid_svg_icons_1.faCircle;
            this.faClipboard = free_solid_svg_icons_1.faClipboard;
            this.faClock = free_solid_svg_icons_1.faClock;
            this.faCalendar = free_regular_svg_icons_1.faCalendar;
            this.faAngleUp = free_solid_svg_icons_1.faAngleUp;
        }
        ProtectiveEquipmentComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            this.route.queryParams.subscribe(function (params) {
                var sessionId = params[queryparameters_1.Queryparameters.SessionId] || 0;
                _this.session = _this.sessionService.getSession(sessionId);
                _this.institutionid = _this.session.department.institutionId;
                if (!_this.session)
                    _this.router.navigate(['']);
            });
        };
        ProtectiveEquipmentComponent_1.prototype.navigateToProtectiveEquipmentRegistrationPage = function (sessionId) {
            this.router.navigate([urls_1.Urls.RegisterProtectiveEquipmentUrl], { queryParams: { sessionId: sessionId } });
        };
        ProtectiveEquipmentComponent_1.prototype.navigateToSentSessions = function () {
            this.router.navigate([urls_1.Urls.SentSessionsUrl]);
        };
        ProtectiveEquipmentComponent_1.prototype.sessionDeletedEventHandler = function (sessionId) {
            this.sessionService.deleteSession(sessionId);
            this.router.navigate([urls_1.Urls.NotSentSessionsUrl]);
        };
        ProtectiveEquipmentComponent_1.prototype.showEquipment = function (protectiveEquipment) {
            if ((protectiveEquipment === null || protectiveEquipment === void 0 ? void 0 : protectiveEquipment.length) > 0) {
                return protectiveEquipment.filter(function (b) { return b.wasUsed; }).map(function (b) { return b.equipmentType.name; }).join(', ');
            }
            return "";
        };
        ProtectiveEquipmentComponent_1.prototype.observationDeletedEventHandler = function ($event) {
            this.session = this.sessionService.getSession(this.session.id);
        };
        ProtectiveEquipmentComponent_1.prototype.sendToCoordinator = function () {
            var _this = this;
            this.sessionSentToServer = true;
            this.sessionService.sendToServer(this.session.id).subscribe(function (res) {
                _this.toastrService.success("Session was sent to coordinator");
                _this.sessionService.deleteSession(_this.session.id);
                _this.sessionIsSentToServer = true;
                _this.sessionSentToServer = false;
            }, function (error) {
                _this.sessionSentToServer = false;
                var message = "Something went wrong while sending session to coordinator: " + ((error === null || error === void 0 ? void 0 : error.error) ? error.error.substr(0, 300) + '...' : error);
                _this.toastrService.error(message, '', { disableTimeOut: true });
            }, function () { return _this.sessionSentToServer = false; });
        };
        ;
        ProtectiveEquipmentComponent_1.prototype.navigateToSentSession = function () {
            this.router.navigate(['/' + urls_1.Urls.SendProtectiveEquipmentSessionUrl], { queryParams: { sessionId: this.session.id } });
        };
        return ProtectiveEquipmentComponent_1;
    }());
    __setFunctionName(_classThis, "ProtectiveEquipmentComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        ProtectiveEquipmentComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return ProtectiveEquipmentComponent = _classThis;
}();
exports.ProtectiveEquipmentComponent = ProtectiveEquipmentComponent;
//# sourceMappingURL=protection-equipment.component.js.map