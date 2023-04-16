using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pacman.Database;
using pacman.Models;

namespace pacman.Controllers;

public class TransactionController : Controller
{
    private readonly ILogger<TransactionController> _logger;
    private readonly PacmanDbContext _db;

    public TransactionController(ILogger<TransactionController> logger, PacmanDbContext database)
    {
        _logger = logger;
        _db = database;
    }

    public IActionResult Index() {

        var result = _db.Transactions!.ToList();
        result.Reverse();

        double balance = 0;

        foreach (var transaction in result)
        {
            balance += transaction.isReceived ? transaction.Amount : - transaction.Amount;
        }

        ViewData["Transactions"] = result;
        ViewData["Balance"] = balance;

        return View();
    }

    public IActionResult Add() => View();

    [HttpPost]
    public IActionResult Add(Transaction transaction) {

        transaction.isReceived = Request.Form["Type"] == "Credit";
        
        _db.Transactions!.Add(transaction);
        _db.SaveChanges();

        return new RedirectResult("/Transaction");
    }

    public IActionResult Delete(int id) {
        Transaction transaction = new Transaction() {Id = id};
        _db.Transactions!.Attach(transaction);
        _db.Transactions!.Remove(transaction);
        _db.SaveChanges();

        return new RedirectResult("/Transaction");
    }

   

    public IActionResult Edit(int id) {
        ViewData["Transaction"] = _db.Transactions!.First(t => t.Id == id);
        return View();
    }

    [HttpPost]
    public IActionResult Update(Transaction transaction) {

        var transactionRecord = _db.Transactions!.FirstOrDefault(t => t.Id == transaction.Id);

        if (transactionRecord == null)
        {
            return new EmptyResult();
        }

        transactionRecord.Amount = transaction.Amount;
        transactionRecord.Name = transaction.Name;
        transactionRecord.isReceived = Request.Form["Type"] == "Credit";

        _db.SaveChanges();

        return new RedirectResult("/Transaction");
    }
}
