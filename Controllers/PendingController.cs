using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pacman.Database;
using pacman.Models;

namespace pacman.Controllers;

public class PendingController : Controller
{
    private readonly ILogger<PendingController> _logger;
    private readonly PacmanDbContext _db;

    public PendingController(ILogger<PendingController> logger, PacmanDbContext database)
    {
        _logger = logger;
        _db = database;
    }

    public IActionResult Index() {

        var result = _db.Pendings!.ToList();
        result.Reverse();

        double balance = 0;

        foreach (var pending in result)
        {
            balance += pending.isReceived ? pending.Amount : - pending.Amount;
        }

        ViewData["Pendings"] = result;
        ViewData["Balance"] = balance;

        return View();
    }

    public IActionResult Add() => View();

    [HttpPost]
    public IActionResult Add(Pending pending) {

        pending.isReceived = Request.Form["Type"] == "Credit";
        
        _db.Pendings!.Add(pending);
        _db.SaveChanges();

        return new RedirectResult("/Pending");
    }

    public IActionResult Delete(int id) {
        Pending pending = new Pending() {Id = id};
        _db.Pendings!.Attach(pending);
        _db.Pendings!.Remove(pending);
        _db.SaveChanges();

        return new RedirectResult("/Pending");
    }

    public IActionResult Settle(int id) {
        Pending? pending = _db.Pendings!.FirstOrDefault(p => p.Id == id);

        if(pending == null) {
            return new RedirectResult("/Pending");
        }

        _db.Pendings!.Attach(pending);
        _db.Pendings!.Remove(pending);

        _db.Transactions!.Add(new Transaction() {
            Amount = pending.Amount,
            Name = pending.Name,
            isReceived = pending.isReceived
        });

        _db.SaveChanges();

        return new RedirectResult("/Pending");
    }

    public IActionResult Edit(int id) {
        ViewData["Pending"] = _db.Pendings!.First(t => t.Id == id);
        return View();
    }

    [HttpPost]
    public IActionResult Update(Pending pending) {

        var pendingRecord = _db.Pendings!.FirstOrDefault(p => p.Id == pending.Id);

        if (pendingRecord == null)
        {
            return new EmptyResult();
        }

        pendingRecord.Amount = pending.Amount;
        pendingRecord.Name = pending.Name;
        pendingRecord.Deadline = pending.Deadline;
        pendingRecord.isReceived = Request.Form["Type"] == "Credit";

        _db.SaveChanges();

        return new RedirectResult("/Pending");
    }
}
